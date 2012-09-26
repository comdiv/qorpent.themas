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
// Original file : ThemaCompiler.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using Qorpent.Themas.Compiler.Pipelines;

namespace Qorpent.Themas.Compiler {
	/// <summary>
	/// </summary>
	public sealed class ThemaCompiler {
		/// <summary>
		/// </summary>
		/// <param name="project"> </param>
		/// <returns> </returns>
		public ThemaCompilerContext Compile(ThemaProject project) {
			lock (this) {
				_pipeline = new ThemaCompilerPipeline();
				var context = new ThemaCompilerContext {Project = project};
				context.UserLog.Debug("context created");
				context.Pipeline = _pipeline;
				PrepareDefaultPipeline(context);
				context.UserLog.Debug("default pipeline created");
				if (null != project.CustomCompiler) {
					context.UserLog.Debug("need call custom compiler InternalSetup");
					project.CustomCompiler.Setup(context);
					context.UserLog.Debug("custom compile InternalSetup called");
				}
				context.UserLog.Trace("preparation finished");
				context.UserLog.Info("compilation started");
				_pipeline.Execute(context);
				context.UserLog.Info("compilation finished");
				if (context.IsComplete) {
					context.UserLog.Info("IsComplete : true");
				}
				else {
					context.UserLog.Error("IsComplete : false (see errors)");
				}
				return context;
			}
		}

		private static void PrepareDefaultPipeline(ThemaCompilerContext context) {
			new CompileToXml().Setup(context);
		}

		private ThemaCompilerPipeline _pipeline;
	}
}