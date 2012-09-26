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
// Original file : ProcessAndThemaRefTest.cs
// Project: Qorpent.Themas.Compiler.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Linq;
using NUnit.Framework;
using Qorpent.Utils.Extensions;

namespace Qorpent.Themas.Compiler.Tests.EcoProcess {
	[TestFixture]
	public class ProcessAndThemaRefTest {
		[Test]
		public void hole_in_stages_prevented() {
			var ctx =
				EPTest.Compile(
					@"
orgnode A
	process Ax
		thema a1, stage = 1
		thema a2, stage = 2
		thema a3, stage = 4
thema A, abst
	activeA = true
	activeB = true
	in A
	in B
A a1
A a2
A a3
");
			Assert.False(ctx.IsComplete);
			Assert.NotNull(ctx.Errors.FirstOrDefault(x => x.ErrorCode == "ER_EPINTEG_2"));
		}

		[Test]
		public void hole_in_stages_prevented_noref() {
			var ctx =
				EPTest.Compile(
					@"
orgnode A
	process Ax
		thema a1, stage = 1
		thema a4, stage = 2
		thema a3, stage = 3
thema A, abst
	activeA = true
	activeB = true
	in A
	in B
A a1
A a2
A a3
",
					x => x.NonResolvedProcessThemaRefIsError = false);
			Assert.False(ctx.IsComplete);
			Assert.NotNull(ctx.Errors.FirstOrDefault(x => x.ErrorCode == "ER_EPINTEG_2"));
		}

		[Test]
		public void prevents_references_to_not_existed_themas() {
			var ctx = EPTest.Compile(@"
orgnode A
	process Ax
		thema At2
thema At
");
			Assert.False(ctx.IsComplete);
			Assert.NotNull(ctx.Errors.FirstOrDefault(x => x.ErrorCode == "ER_EPI05"));
		}

		[Test]
		public void process_in_dependency_injected() {
			var ctx =
				EPTest.Compile(
					@"
orgnode A
	process Ax
		thema a1, outview, outlock
	process Ax2
		in Ax
		thema a2, outview, outlock
	process Ax3
		in Ax2
		thema a3

thema A, abst
	activeA = true
	activeB = true
	in A
	in B
A a1
A a2
A a3
");
			var p = ctx.EcoProcessIndex.Index["Ax"];
			var t1 = ctx.Themas["a1"];
			var t2 = ctx.Themas["a2"];
			var t3 = ctx.Themas["a3"];
			var f1a = t1.Items["A.in"];
			var f1b = t1.Items["B.in"];
			var f2a = t2.Items["A.in"];
			var f2b = t2.Items["B.in"];
			var f3a = t3.Items["A.in"];
			var f3b = t3.Items["B.in"];
			Assert.Null(f1a.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend"));
			Assert.Null(f1b.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend"));
			Assert.NotNull(f2a.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend" && x.Attr("code") == "a1.A.in"));
			Assert.NotNull(f2b.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend" && x.Attr("code") == "a1.B.in"));
			Assert.NotNull(f3a.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend" && x.Attr("code") == "a2.A.in"));
			Assert.NotNull(f3b.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend" && x.Attr("code") == "a2.B.in"));
			Assert.Null(f3a.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend" && x.Attr("code") == "a1.A.in"));
			Assert.Null(f3b.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend" && x.Attr("code") == "a1.B.in"));
		}

		[Test]
		public void process_in_dependency_injected_with_group() {
			var ctx =
				EPTest.Compile(
					@"
orgnode A
	process Ax
		thema a1, outview, outlock
	process Ax2
		in Ax
		thema a2, B, outview, outlock
	process Ax3
		in Ax2
		thema a3

thema A, abst
	activeA = true
	activeB = true
	in A
	in B
A a1
A a2
A a3
");
			var p = ctx.EcoProcessIndex.Index["Ax"];
			var t1 = ctx.Themas["a1"];
			var t2 = ctx.Themas["a2"];
			var t3 = ctx.Themas["a3"];
			var f1a = t1.Items["A.in"];
			var f1b = t1.Items["B.in"];
			var f2a = t2.Items["A.in"];
			var f2b = t2.Items["B.in"];
			var f3a = t3.Items["A.in"];
			var f3b = t3.Items["B.in"];
			Assert.Null(f1a.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend"));
			Assert.Null(f1b.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend"));
			Assert.Null(f2a.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend" && x.Attr("code") == "a1.A.in"));
			Assert.NotNull(f2b.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend" && x.Attr("code") == "a1.B.in"));
			Assert.Null(f3a.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend" && x.Attr("code") == "a2.A.in"));
			Assert.NotNull(f3b.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend" && x.Attr("code") == "a2.B.in"));
			Assert.Null(f3a.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend" && x.Attr("code") == "a1.A.in"));
			Assert.Null(f3b.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend" && x.Attr("code") == "a1.B.in"));
		}


		[Test]
		public void process_in_dependency_not_injected_with_no_outlock() {
			var ctx =
				EPTest.Compile(
					@"
orgnode A
	process Ax
		thema a1, outview
	process Ax2
		in Ax
		thema a2, outview, outlock
	process Ax3
		in Ax2
		thema a3

thema A, abst
	activeA = true
	activeB = true
	in A
	in B
A a1
A a2
A a3
");
			var p = ctx.EcoProcessIndex.Index["Ax"];
			var t1 = ctx.Themas["a1"];
			var t2 = ctx.Themas["a2"];
			var t3 = ctx.Themas["a3"];
			var f1a = t1.Items["A.in"];
			var f1b = t1.Items["B.in"];
			var f2a = t2.Items["A.in"];
			var f2b = t2.Items["B.in"];
			var f3a = t3.Items["A.in"];
			var f3b = t3.Items["B.in"];
			Assert.Null(f1a.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend"));
			Assert.Null(f1b.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend"));
			Assert.Null(f2a.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend" && x.Attr("code") == "a1.A.in"));
			Assert.Null(f2b.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend" && x.Attr("code") == "a1.B.in"));
			Assert.NotNull(f3a.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend" && x.Attr("code") == "a2.A.in"));
			Assert.NotNull(f3b.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend" && x.Attr("code") == "a2.B.in"));
			Assert.Null(f3a.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend" && x.Attr("code") == "a1.A.in"));
			Assert.Null(f3b.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend" && x.Attr("code") == "a1.B.in"));
		}

		[Test]
		public void provides_process_to_thema_OUTVIEWREPORT() {
			var ctx =
				EPTest.Compile(
					@"
orgnode A
	process Ax
		thema At, outview
thema At
	activeA = true
	activeB = true
	in A
	out aA
	in B
	out aB
");
			var p = ctx.EcoProcessIndex.Index["Ax"];
			var t = ctx.Themas["At"];
			var ra = t.Items["aA.out"];
			var fa = t.Items["A.in"];
			var rb = t.Items["aB.out"];
			var fb = t.Items["B.in"];
			Assert.True(t.IsUnderEcoProcess);
			Assert.AreEqual("At", p.ThemaRefs.First().Thema.Code);
			Assert.AreEqual(p.Code, t.OwnerProcessA);
			Assert.AreEqual(p.Code, t.OwnerProcessB);
			Assert.AreEqual(p.Code, t.EcoProcess);
			Assert.AreEqual("Ax_OWN; Ax_VIEW; At_OWN; At_VIEW", t.Role);
			Assert.AreEqual("Ax_OWN; At_FORM_A_OWN", fa.Attr("role"));
			Assert.AreEqual("Ax_VIEW; Ax_OWN; At_REPORT_VIEW", ra.Attr("role"));
			Assert.AreEqual("Ax_OWN; At_FORM_B_OWN", fb.Attr("role"));
			Assert.AreEqual("Ax_VIEW; Ax_OWN; At_REPORT_VIEW", rb.Attr("role"));
		}

		[Test]
		public void provides_process_to_thema_OUTVIEWREPORT_A_GROUP() {
			var ctx =
				EPTest.Compile(
					@"
orgnode A
	process Ax
		thema At, A, outview
thema At
	activeA = true
	activeB = true
	in A
	out aA
	in B
	out aB
");
			var p = ctx.EcoProcessIndex.Index["Ax"];
			var t = ctx.Themas["At"];
			var ra = t.Items["aA.out"];
			var fa = t.Items["A.in"];
			var rb = t.Items["aB.out"];
			var fb = t.Items["B.in"];
			Assert.True(t.IsUnderEcoProcess);
			Assert.AreEqual("At", p.ThemaRefs.First().Thema.Code);
			Assert.AreEqual(p.Code, t.OwnerProcessA);
			Assert.AreEqual("", t.OwnerProcessB);
			Assert.AreEqual(p.Code, t.EcoProcess);
			Assert.AreEqual("Ax_OWN; Ax_VIEW; At_OWN; At_VIEW", t.Role);
			Assert.AreEqual("Ax_OWN; At_FORM_A_OWN", fa.Attr("role"));
			Assert.AreEqual("Ax_VIEW; Ax_OWN; At_REPORT_VIEW", ra.Attr("role"));
			Assert.AreEqual("At_FORM_B_OWN", fb.Attr("role"));
			Assert.AreEqual("Ax_VIEW; Ax_OWN; At_REPORT_VIEW", rb.Attr("role"));
		}

		[Test]
		public void provides_process_to_thema_OUTVIEWREPORT_A_GROUP_2_PROCESS() {
			var ctx =
				EPTest.Compile(
					@"
orgnode A
	process Ax
		thema At, A, outview
	process Ax2
		thema At, B, outview
thema At
	activeA = true
	activeB = true
	in A
	out aA
	in B
	out aB
");
			var p = ctx.EcoProcessIndex.Index["Ax"];
			var p2 = ctx.EcoProcessIndex.Index["Ax2"];
			var t = ctx.Themas["At"];
			var ra = t.Items["aA.out"];
			var fa = t.Items["A.in"];
			var rb = t.Items["aB.out"];
			var fb = t.Items["B.in"];
			Assert.True(t.IsUnderEcoProcess);
			Assert.AreEqual("At", p.ThemaRefs.First().Thema.Code);
			Assert.AreEqual(p.Code, t.OwnerProcessA);
			Assert.AreEqual(p2.Code, t.OwnerProcessB);
			Assert.AreEqual(p.Code + "; " + p2.Code, t.EcoProcess);
			Assert.AreEqual("Ax_OWN; Ax_VIEW; Ax2_OWN; Ax2_VIEW; At_OWN; At_VIEW", t.Role);
			Assert.AreEqual("Ax_OWN; At_FORM_A_OWN", fa.Attr("role"));
			Assert.AreEqual("Ax_VIEW; Ax_OWN; Ax2_VIEW; Ax2_OWN; At_REPORT_VIEW", ra.Attr("role"));
			Assert.AreEqual("Ax2_OWN; At_FORM_B_OWN", fb.Attr("role"));
			Assert.AreEqual("Ax_VIEW; Ax_OWN; Ax2_VIEW; Ax2_OWN; At_REPORT_VIEW", rb.Attr("role"));
		}

		[Test]
		public void provides_process_to_thema_SIMPLE() {
			var ctx =
				EPTest.Compile(
					@"
orgnode A
	process Ax
		thema At
thema At
	activeA = true
	activeB = true
	in A
	out aA
	in B
	out aB
");
			var p = ctx.EcoProcessIndex.Index["Ax"];
			var t = ctx.Themas["At"];
			var ra = t.Items["aA.out"];
			var fa = t.Items["A.in"];
			var rb = t.Items["aB.out"];
			var fb = t.Items["B.in"];
			Assert.True(t.IsUnderEcoProcess);
			Assert.AreEqual("At", p.ThemaRefs.First().Thema.Code);
			Assert.AreEqual(p.Code, t.OwnerProcessA);
			Assert.AreEqual(p.Code, t.OwnerProcessB);
			Assert.AreEqual(p.Code, t.EcoProcess);
			Assert.AreEqual("Ax_OWN; Ax_VIEW; At_OWN; At_VIEW", t.Role);
			Assert.AreEqual("Ax_OWN; At_FORM_A_OWN", fa.Attr("role"));
			Assert.AreEqual("Ax_OWN; At_REPORT_VIEW", ra.Attr("role"));
			Assert.AreEqual("Ax_OWN; At_FORM_B_OWN", fb.Attr("role"));
			Assert.AreEqual("Ax_OWN; At_REPORT_VIEW", rb.Attr("role"));
		}

		[Test]
		public void stage_dependency_injected() {
			var ctx =
				EPTest.Compile(
					@"
orgnode A
	process Ax
		thema a1, stage = 1
		thema a2, stage = 2
		thema a3, stage = 3
thema A, abst
	activeA = true
	activeB = true
	in A
	in B
A a1
A a2
A a3
");
			var p = ctx.EcoProcessIndex.Index["Ax"];
			var t1 = ctx.Themas["a1"];
			var t2 = ctx.Themas["a2"];
			var t3 = ctx.Themas["a3"];
			var f1a = t1.Items["A.in"];
			var f1b = t1.Items["B.in"];
			var f2a = t2.Items["A.in"];
			var f2b = t2.Items["B.in"];
			var f3a = t3.Items["A.in"];
			var f3b = t3.Items["B.in"];
			Assert.Null(f1a.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend"));
			Assert.Null(f1b.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend"));
			Assert.NotNull(f2a.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend" && x.Attr("code") == "a1.A.in"));
			Assert.NotNull(f2b.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend" && x.Attr("code") == "a1.B.in"));
			Assert.NotNull(f3a.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend" && x.Attr("code") == "a2.A.in"));
			Assert.NotNull(f3b.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend" && x.Attr("code") == "a2.B.in"));
			Assert.Null(f3a.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend" && x.Attr("code") == "a1.A.in"));
			Assert.Null(f3b.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend" && x.Attr("code") == "a1.B.in"));
		}

		[Test]
		public void stage_dependency_injected_with_group() {
			var ctx =
				EPTest.Compile(
					@"
orgnode A
	process Ax
		thema a1, stage = 1
		thema a2, B, stage = 2
		thema a3, stage = 3
thema A, abst
	activeA = true
	activeB = true
	in A
	in B
A a1
A a2
A a3
");
			var p = ctx.EcoProcessIndex.Index["Ax"];
			var t1 = ctx.Themas["a1"];
			var t2 = ctx.Themas["a2"];
			var t3 = ctx.Themas["a3"];
			var f1a = t1.Items["A.in"];
			var f1b = t1.Items["B.in"];
			var f2a = t2.Items["A.in"];
			var f2b = t2.Items["B.in"];
			var f3a = t3.Items["A.in"];
			var f3b = t3.Items["B.in"];
			Assert.Null(f1a.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend"));
			Assert.Null(f1b.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend"));
			Assert.Null(f2a.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend" && x.Attr("code") == "a1.A.in"));
			Assert.NotNull(f2b.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend" && x.Attr("code") == "a1.B.in"));
			Assert.Null(f3a.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend" && x.Attr("code") == "a2.A.in"));
			Assert.NotNull(f3b.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend" && x.Attr("code") == "a2.B.in"));
			Assert.Null(f3a.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend" && x.Attr("code") == "a1.A.in"));
			Assert.Null(f3b.Elements().FirstOrDefault(x => x.Name.LocalName == "lockdepend" && x.Attr("code") == "a1.B.in"));
		}

		[Test]
		public void warning_mode_to_prevent_references_to_not_existed_themas() {
			var ctx = EPTest.Compile(@"
orgnode A
	process Ax
		thema At2
thema At
",
			                         x => x.NonResolvedProcessThemaRefIsError = false);
			Assert.True(ctx.IsComplete);
			Assert.NotNull(ctx.Errors.FirstOrDefault(x => x.ErrorCode == "WR_EPI05"));
		}
	}
}