#region LICENSE

// Copyright 2007-2012 Comdiv (F. Sadykov) - http://code.google.com/u/fagim.sadykov/
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
// http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Solution: Qorpent
// Original file : ThemaCompilerPipeline.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Themas.Compiler.Steps;

namespace Qorpent.Themas.Compiler {
	/// <summary>
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class ThemaCompilerPipeline : List<IThemaCompilerStep> {
		/// <summary>
		/// 	Executes the specified context.
		/// </summary>
		/// <param name="context"> The context. </param>
		/// <remarks>
		/// </remarks>
		public void Execute(ThemaCompilerContext context) {
			lock (this) {
				var stepindex = 0;
				context.Pipeline = this;
				foreach (var step in this) {
					try {
						context.UserLog.Trace("enter step " + stepindex + " " + step.GetType().Name);
						context.StepIndex = stepindex;
						step.Process(context);
						context.UserLog.Debug("end step " + stepindex + " " + step.GetType().Name);
						stepindex++;
					}
					catch (Exception ex) {
						context.UserLog.Error("error step " + stepindex + " " + step.GetType().Name);
						context.IsComplete = false;
						context.Errors.Add(
							new ThemaCompilerError
								{
									Exception = ex,
									Managed = false,
									Message = "general error in " + step.GetType().Name + " step",
									Level = ErrorLevel.Fatal,
								}
							);
						new GenerateHtmlReportStep().Process(context);
						return;
					}
					var errorlevels = context.Errors.Select(x => x.Level).ToArray();
					if (!errorlevels.Any()) {
						continue;
					}
					var maxlevel = errorlevels.Max();
					if (maxlevel < context.Project.ErrorLevel) {
						continue;
					}
					context.IsComplete = false;
					new GenerateHtmlReportStep().Process(context);
					return;
				}
				context.IsComplete = true;
			}
		}
	}
}