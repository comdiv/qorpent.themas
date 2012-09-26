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
// Original file : ThemaCompilerError.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.Themas.Compiler {
	// ReSharper disable MemberCanBePrivate.Global
	/// <summary>
	/// 	compiler error descriptor
	/// </summary>
	/// <remarks>
	/// </remarks>
	[Serialize]
	public class ThemaCompilerError {
		/// <summary>
		/// 	error code
		/// </summary>
		/// <value> The error code. </value>
		/// <remarks>
		/// </remarks>
		[Serialize] public string ErrorCode { get; set; }

		/// <summary>
		/// 	severity level of error
		/// </summary>
		/// <value> The level. </value>
		/// <remarks>
		/// </remarks>
		[Serialize] public ErrorLevel Level { get; set; }

		/// <summary>
		/// 	step, where error occured
		/// </summary>
		/// <value> The step. </value>
		/// <remarks>
		/// </remarks>
		[IgnoreSerialize] public ThemaCompilerStep Step { get; set; }


		/// <summary>
		/// 	internal exception
		/// </summary>
		/// <value> The exception. </value>
		/// <remarks>
		/// </remarks>
		[Serialize] public Exception Exception { get; set; }

		/// <summary>
		/// 	message
		/// </summary>
		/// <value> The message. </value>
		/// <remarks>
		/// </remarks>
		[Serialize] public string Message { get; set; }

		/// <summary>
		/// 	file, where error occured
		/// </summary>
		/// <value> The source file. </value>
		/// <remarks>
		/// </remarks>
		[Serialize] public string SourceFile { get; set; }

		/// <summary>
		/// 	line, where error occured
		/// </summary>
		/// <value> The line. </value>
		/// <remarks>
		/// </remarks>
		[Serialize] public int Line { get; set; }

		/// <summary>
		/// 	column, where error occured
		/// </summary>
		/// <value> The column. </value>
		/// <remarks>
		/// </remarks>
		[Serialize] public int Column { get; set; }

		/// <summary>
		/// 	indicate, that system provides self solution for this error
		/// </summary>
		/// <value> <c>true</c> if managed; otherwise, <c>false</c> . </value>
		/// <remarks>
		/// </remarks>
		[Serialize] public bool Managed { get; set; }

		/// <summary>
		/// 	Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns> A <see cref="System.String" /> that represents this instance. </returns>
		/// <remarks>
		/// </remarks>
		public override string ToString() {
			return string.Format(@"{0} {1}{2}{3} {4} {5} {6}"
			                     , Level
			                     , ErrorCode
			                     , null == Step ? "" : " in " + Step.GetType().Name
			                     ,
			                     SourceFile.IsEmpty()
				                     ? ""
				                     : " at " + SourceFile + " (" + Line + ", " + Column + ")"
			                     , Message
			                     , Managed ? "" : "GENERAL"
			                     , null == Exception ? "" : "\r\n" + Exception
				);
		}
	}
}

// ReSharper restore MemberCanBePrivate.Global