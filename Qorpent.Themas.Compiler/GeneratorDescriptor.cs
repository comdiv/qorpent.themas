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
// Original file : GeneratorDescriptor.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Linq;
using System.Xml.Linq;

namespace Qorpent.Themas.Compiler {
	/// <summary>
	/// 	descriptror for custom generator class
	/// </summary>
	public class GeneratorDescriptor {
		/// <summary>
		/// 	creates new
		/// </summary>
		/// <param name="code"> </param>
		/// <param name="type"> </param>
		public GeneratorDescriptor(string code, string type) {
			_code = code;
			_typename = type;
		}

		/// <summary>
		/// 	checkout type availability
		/// </summary>
		public bool IsValid {
			get { return Type != null; }
		}

		/// <summary>
		/// 	unique code
		/// </summary>
		public string Code {
			get { return _code; }
		}

// ReSharper disable MemberCanBePrivate.Global
		/// <summary>
		/// 	type of internal generator
		/// </summary>
		public Type Type { get; private set; }

		// ReSharper restore MemberCanBePrivate.Global
		/// <summary>
		/// 	typename of internal generator
		/// </summary>
		public string Typename {
			get { return _typename; }
		}

		/// <summary>
		/// 	prepares generator for usage
		/// </summary>
		public void PrepareType() {
			Type = Type.GetType(_typename, true, true);
			if (!typeof (IThemaXmlGenerator).IsAssignableFrom(Type)) {
				throw new Exception("given type " + _typename + " does not support IThemaXmlGenerator interface");
			}
		}

		/// <summary>
		/// 	executes generator
		/// </summary>
		/// <param name="context"> </param>
		/// <param name="callelement"> </param>
		public void Execute(ThemaCompilerContext context, XElement callelement) {
			var generator = Activator.CreateInstance(Type) as IThemaXmlGenerator;
			if (generator is IThemaCompileTimeGenerator) {
				callelement.ReplaceWith(((IThemaCompileTimeGenerator) generator).Generate(callelement, context).ToArray());
			}
			else {
				if (generator != null) {
					callelement.ReplaceWith(generator.Generate(callelement).ToArray());
				}
			}
		}

		private readonly string _code;
		private readonly string _typename;
	}
}