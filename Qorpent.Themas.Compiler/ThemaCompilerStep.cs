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
// Original file : ThemaCompilerStep.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Log;

namespace Qorpent.Themas.Compiler {
	/// <summary>
	/// 	base class for thema compiler
	/// </summary>
	/// <remarks>
	/// </remarks>
	public abstract class ThemaCompilerStep : IThemaCompilerStep {
		/// <summary>
		/// 	Gets the UserLog.
		/// </summary>
		/// <remarks>
		/// </remarks>
		protected IUserLog UserLog {
			get { return _userLog ?? (_userLog = Context.UserLog); }
		}


		/// <summary>
		/// 	Processes the specified CTX.
		/// </summary>
		/// <param name="ctx"> The CTX. </param>
		/// <remarks>
		/// </remarks>
		public void Process(ThemaCompilerContext ctx) {
			Context = ctx;
			InternalProcess();
		}


		/// <summary>
		/// 	Internals the process.
		/// </summary>
		/// <remarks>
		/// </remarks>
		protected abstract void InternalProcess();

		/// <summary>
		/// 	Visits the text and attributes.
		/// </summary>
		/// <param name="e"> The e. </param>
		/// <param name="visitor"> The visitor. </param>
		/// <param name="filter"> The filter. </param>
		/// <remarks>
		/// </remarks>
		protected static void VisitTextAndAttributes(XElement e, Func<XObject, string, string> visitor,
		                                             Func<XElement, bool> filter = null) {
			foreach (var a in e.Attributes()) {
				var result = visitor(a, a.Value);
				if (!ReferenceEquals(result, a.Value)) {
					a.Value = result;
				}
			}
			foreach (var xText in e.Nodes().OfType<XText>()) {
				var result = visitor(xText, xText.Value);
				if (!ReferenceEquals(result, xText.Value)) {
					xText.Value = result;
				}
			}
			foreach (var el in e.Elements().Where(el => null == filter || filter(el))) {
				VisitTextAndAttributes(el, visitor, filter);
			}
		}

		/// <summary>
		/// 	Adds the error.
		/// </summary>
		/// <param name="level"> The level. </param>
		/// <param name="message"> The message. </param>
		/// <param name="errorCode"> The error code. </param>
		/// <param name="ex"> The ex. </param>
		/// <param name="file"> The file. </param>
		/// <param name="line"> The line. </param>
		/// <param name="column"> The column. </param>
		/// <remarks>
		/// </remarks>
		protected void AddError(ErrorLevel level, string message, string errorCode, Exception ex = null, string file = null,
		                        int line = 0, int column = 0) {
			Context.Errors.Add(
				new ThemaCompilerError
					{
						ErrorCode = errorCode,
						Step = this,
						Exception = ex,
						Message = message,
						Managed = true,
						SourceFile = file,
						Line = line,
						Column = column,
						Level = level,
					}
				);
		}

		/// <summary>
		/// </summary>
		protected ThemaCompilerContext Context;

		/// <summary>
		/// </summary>
		private IUserLog _userLog;
	}
}