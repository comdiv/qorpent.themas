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
// Original file : ThemaLinkType.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;

namespace Qorpent.Themas.Compiler {
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
	/// <summary>
	/// </summary>
	public class ThemaLinkType
// ReSharper restore ClassWithVirtualMembersNeverInherited.Global
	{
		/// <summary>
		/// </summary>
		public string Code { get; set; }

		/// <summary>
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// </summary>
		public string BackName { get; set; }

		/// <summary>
		/// </summary>
		public bool Singleton { get; set; }

		/// <summary>
		/// </summary>
		public bool Inheritable { get; set; }

		/// <summary>
		/// </summary>
		public bool RequireResolution { get; set; }

		/// <summary>
		/// </summary>
		public string RequireAttribute { get; set; }

// ReSharper disable MemberCanBePrivate.Global
		/// <summary>
		/// </summary>
		public Func<ThemaLink, ThemaDescriptor, ThemaCompilerError> OnCustomValidation { get; set; }

// ReSharper restore MemberCanBePrivate.Global
		/// <summary>
		/// </summary>
		public virtual ThemaCompilerError CustomValidation(ThemaLink link, ThemaDescriptor thema) {
			return null != OnCustomValidation ? OnCustomValidation(link, thema) : null;
		}
	}
}