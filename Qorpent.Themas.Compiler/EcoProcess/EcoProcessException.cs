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
// Original file : EcoProcessException.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Themas.Compiler.EcoProcess {
	/// <summary>
	/// 	general exception on ecoprocess module processing
	/// </summary>
	/// <remarks>
	/// </remarks>
	[Serializable]
	public class EcoProcessException : Exception {
		//
		// For guidelines regarding the creation of new exception types, see
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
		// and
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
		//

		/// <summary>
		/// 	Initializes a new instance of the <see cref="EcoProcessException" /> class.
		/// </summary>
		/// <param name="message"> The message. </param>
		/// <param name="file"> The file. </param>
		/// <param name="line"> The line. </param>
		/// <remarks>
		/// </remarks>
		public EcoProcessException(string message, string file = "", int line = 0) : base(message) {
			File = file;
			Line = line;
		}

		/// <summary>
		/// 	Initializes a new instance of the <see cref="T:System.Exception" /> class with serialized data.
		/// </summary>
		/// <param name="info"> The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown. </param>
		/// <param name="context"> The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination. </param>
		/// <exception cref="T:System.ArgumentNullException">The
		/// 	<paramref name="info" />
		/// 	parameter is null.</exception>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or
		/// 	<see cref="P:System.Exception.HResult" />
		/// 	is zero (0).</exception>
		/// <remarks>
		/// </remarks>
		protected EcoProcessException(
			SerializationInfo info,
			StreamingContext context) : base(info, context) {}

		/// <summary>
		/// 	Initializes a new instance of the <see cref="EcoProcessException" /> class.
		/// </summary>
		/// <param name="message"> The message. </param>
		/// <param name="source"> The source. </param>
		/// <remarks>
		/// </remarks>
		public EcoProcessException(string message, XElement source) : this(message, source.Describe()) {}

		/// <summary>
		/// 	Initializes a new instance of the <see cref="EcoProcessException" /> class.
		/// </summary>
		/// <param name="message"> The message. </param>
		/// <param name="describe"> The describe. </param>
		/// <remarks>
		/// </remarks>
		private EcoProcessException(string message, XmlExtensions.XmlElementDescriptor describe)
			: this(message, describe.File, describe.Line) {}

		/// <summary>
		/// 	Gets or sets the file.
		/// </summary>
		/// <value> The file. </value>
		/// <remarks>
		/// </remarks>
		public string File { get; set; }

		/// <summary>
		/// 	Gets or sets the line.
		/// </summary>
		/// <value> The line. </value>
		/// <remarks>
		/// </remarks>
		public int Line { get; set; }
	}
}