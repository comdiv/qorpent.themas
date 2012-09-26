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
// Original file : EmptyThemaCompilerTest.cs
// Project: Qorpent.Themas.Compiler.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using NUnit.Framework;
using Qorpent.Themas.Compiler.Pipelines;

namespace Qorpent.Themas.Compiler.Tests {
	[TestFixture(Description =
		@"Проверяет общую работу инфраструктуры компилятора,
без реальной рабочей нагрузки, только общие моменты")]
	public class EmptyThemaCompilerTest : ThemaCompilerTestBase {
		private class managederror : ThemaCompilerStep, IThemaCompilerSetup {
			public void Setup(ThemaCompilerContext context) {
				context.Pipeline.Add(this);
			}


			protected override void InternalProcess() {
				AddError(ErrorLevel.Error, "error", "TE001", null, "file", 1, 2);
			}
		}

		private class error : Empty, IThemaCompilerStep {
			public void Process(ThemaCompilerContext ctx) {
				throw new Exception("error");
			}


			protected override void InternalSetup(ThemaCompilerContext context) {
				base.InternalSetup(context);
				context.Pipeline.Add(this);
			}
		}

		private class warrning : ThemaCompilerStep, IThemaCompilerSetup {
			public void Setup(ThemaCompilerContext context) {
				context.Pipeline.Add(this);
			}


			protected override void InternalProcess() {
				AddError(ErrorLevel.Warning, "warrning", "TW001", null, "file", 2, 3);
			}
		}

		[Test]
		public void empty_test() {
			var result = execute<Empty>();
			Assert.True(result.IsComplete);
		}

		[Test]
		public void error_test() {
			var result = execute<error>();
			Assert.False(result.IsComplete);
		}

		[Test]
		public void managed_error_test() {
			var result = execute<managederror>();
			Assert.False(result.IsComplete);
		}

		[Test]
		public void warning_aserror_test() {
			var result = execute<warrning>(new ThemaProject {ErrorLevel = ErrorLevel.Warning});
			Assert.False(result.IsComplete);
		}

		[Test]
		public void warning_test() {
			var result = execute<warrning>();
			Assert.True(result.IsComplete);
		}
	}
}