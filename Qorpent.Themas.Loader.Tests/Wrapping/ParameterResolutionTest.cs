using System;
using System.Collections.Generic;
using Comdiv.ThemaLoader.Wrap;
using NUnit.Framework;

namespace Comdiv.ThemaLoader.Test.Wrapping {
	[TestFixture]
	public class ParameterResolutionTest:WTBaseTest {
		private IThemaItemWrapper item;

		public ParameterResolutionTest() {
			this.code = @"
thema lib, islibrary=true
	out l1.out
		var A, type=bool : true
		param B, target=condition : bdefault
		param C : cdefault
		param D : '[[C]] [[iattr]] [[tattr]] [[S]]'
		param I type=int : 22
		param DT type=date : '2012-01-03'
		param DT2 type=date : '03.01.2012'
		param noadmin, exrole=ADMIN : noadmin
		param adminonly, role=ADMIN : admin
		
thema X, myname
	tattr = 3
	out A.out, iattr = 2
		uselib lib.l1.out
		var C : 'acdefault [[name]]'
		var X target=condition: xdefault
";
		}
		[SetUp]
		public void setup() {
			this.item = getitem();
		}

		IThemaItemWrapper getitem(string usr = "test\\admin",WrapContext context = null) {
			return load(usr).WrapReport("X.A",context);
		}

		[Test]
		public void admin_security_test() {
			Assert.False(item.ParameterValues.ContainsKey("noadmin"));
			Assert.True(item.ParameterValues.ContainsKey("adminonly"));
		}


		[Test]
		public void bug_no_conditions_loaded() {
			item = getitem("test\\admin",
					new WrapContext { UserParameters = new Dictionary<string, string> { { "condition", "test1,test2" } } });
			Assert.True(this.item.Conditions.Contains("test1"));
			Assert.True(this.item.Conditions.Contains("test2"));
		}

		[Test]
		public void no_admin_security_test() {
			item = getitem("test\\usr");
			Assert.True(item.ParameterValues.ContainsKey("noadmin"));
			Assert.False(item.ParameterValues.ContainsKey("adminonly"));
		}

		[Test(Description = "проверяем сам факт наличия параметров в выдаче")]
		public void all_parameters_loaded() {
			Assert.True(item.AllParameters.ContainsKey("A"));
			Assert.True(item.AllParameters.ContainsKey("B"));
			Assert.True(item.AllParameters.ContainsKey("C"));
			Assert.True(item.AllParameters.ContainsKey("D"));
			Assert.True(item.AllParameters.ContainsKey("X"));
			Assert.True(item.AllParameters.ContainsKey("condition"));
		}
		[Test(Description = "проверяем сам факт наличия параметров в выдаче (c контекстом)")]
		public void all_parameters_loaded_with_context()
		{
			usrload();
			Assert.True(item.AllParameters.ContainsKey("A"));
			Assert.True(item.AllParameters.ContainsKey("B"));
			Assert.True(item.AllParameters.ContainsKey("C"));
			Assert.True(item.AllParameters.ContainsKey("D"));
			Assert.True(item.AllParameters.ContainsKey("X"));
			Assert.True(item.AllParameters.ContainsKey("S"));
			Assert.True(item.AllParameters.ContainsKey("U"));
			Assert.True(item.AllParameters.ContainsKey("condition"));
		}

		[Test]
		public void raw_parameter_values_resolved() {
			Assert.AreEqual("true",Av.StringValue);
			Assert.AreEqual("bdefault", Bv.StringValue);
			Assert.AreEqual("acdefault [[name]]", Cv.StringValue);
			Assert.AreEqual("[[C]] [[iattr]] [[tattr]] [[S]]", Dv.StringValue);
			Assert.AreEqual("xdefault", Xv.StringValue);
			Assert.AreEqual(";bdefault;xdefault", conditionv.StringValue);
		}

		[Test]
		public void raw_parameter_values_resolved_context()
		{
			usrload();
			Assert.AreEqual("userA", Av.StringValue);
			Assert.AreEqual("savedB", Bv.StringValue);
			Assert.AreEqual("acdefault [[name]]", Cv.StringValue);
			Assert.AreEqual("[[C]] [[iattr]] [[tattr]] [[S]]", Dv.StringValue);
			Assert.AreEqual("userX", Xv.StringValue);
			Assert.AreEqual("savedOnly", Sv.StringValue);
			Assert.AreEqual("useronly", Uv.StringValue);
			Assert.AreEqual(";savedB;userX", conditionv.StringValue);
		}

		[Test]
		public void resolved_parameter_values_evaluated()
		{
			Assert.AreEqual("true", Av.ResolvedStringValue);
			Assert.AreEqual("bdefault", Bv.ResolvedStringValue);
			Assert.AreEqual("acdefault myname", Cv.ResolvedStringValue);
			Assert.AreEqual("acdefault myname 2 3 ", Dv.ResolvedStringValue);
			Assert.AreEqual("xdefault", Xv.ResolvedStringValue);
			Assert.AreEqual(";bdefault;xdefault", conditionv.ResolvedStringValue);
		}

		[Test]
		public void resolved_parameter_values_evaluated_context()
		{
			usrload();
			Assert.AreEqual("userA", Av.ResolvedStringValue);
			Assert.AreEqual("savedB", Bv.ResolvedStringValue);
			Assert.AreEqual("acdefault myname", Cv.ResolvedStringValue);
			Assert.AreEqual("acdefault myname 2 3 savedOnly", Dv.ResolvedStringValue);
			Assert.AreEqual("userX", Xv.ResolvedStringValue);
			Assert.AreEqual("savedOnly", Sv.ResolvedStringValue);
			Assert.AreEqual("useronly", Uv.ResolvedStringValue);
			Assert.AreEqual(";savedB;userX", conditionv.ResolvedStringValue);
		}

		[Test]
		public void final_parameter_values_evaluated()
		{
			Assert.AreEqual(true, Av.Value);
			Assert.AreEqual("bdefault", Bv.Value);
			Assert.AreEqual("acdefault myname", Cv.Value);
			Assert.AreEqual("acdefault myname 2 3 ", Dv.Value);
			Assert.AreEqual(22, Iv.Value);
			Assert.AreEqual(new DateTime(2012,1,3), DTv.Value);
			Assert.AreEqual(new DateTime(2012, 1, 3), DT2v.Value);
			Assert.AreEqual("xdefault", Xv.Value);
			Assert.AreEqual(";bdefault;xdefault", conditionv.Value);

		}

		[Test]
		public void condition_string_evaluated() {
			Assert.True(item.EvalConditionString("bdefault & xdefault"));
			Assert.False(item.EvalConditionString("bdefault & !xdefault"));
			Assert.True(item.EvalConditionString("bdefault & A"));
			Assert.False(item.EvalConditionString("bdefault & U"));
			Assert.True(item.EvalConditionString("bdefault & name = 'myname'"));
			Assert.False(item.EvalConditionString("bdefault & name != 'myname'"));
		}

		[Test]
		public void conditions_evaluated() {
			Assert.AreEqual(2,item.Conditions.Count);
			Assert.True(item.Conditions.Contains("bdefault"));
			Assert.True(item.Conditions.Contains("xdefault"));
		}
		[Test]
		public void final_parameter_values_evaluated_context()
		{
			usrload();
			Assert.AreEqual(true, Av.Value);
			Assert.AreEqual("savedB", Bv.Value);
			Assert.AreEqual("acdefault myname", Cv.Value);
			Assert.AreEqual("acdefault myname 2 3 savedOnly", Dv.Value);
			Assert.AreEqual(22, Iv.Value);
			Assert.AreEqual(new DateTime(2012, 1, 3), DTv.Value);
			Assert.AreEqual(new DateTime(2012, 1, 3), DT2v.Value);
			Assert.AreEqual("userX", Xv.Value);
			Assert.AreEqual("savedOnly", Sv.Value);
			Assert.AreEqual("useronly", Uv.Value);
			Assert.AreEqual(";savedB;userX", conditionv.Value);
		}
		[Test]
		public void conditions_evaluated_context()
		{
			usrload();
			Assert.AreEqual(2, item.Conditions.Count);
			Assert.True(item.Conditions.Contains("savedB"));
			Assert.True(item.Conditions.Contains("userX"));
		}

		public ThemaItemParameter param(string name) {
			return item.AllParameters[name];
		}


		public ParameterValue paramv(string name)
		{
			return item.ParameterValues[name];
		}
		public ThemaItemParameter A { get { return param("A"); } }
		public ThemaItemParameter B { get { return param("B"); } }
		public ThemaItemParameter C { get { return param("C"); } }
		public ThemaItemParameter D { get { return param("D"); } }
		public ThemaItemParameter I { get { return param("I"); } }
		public ThemaItemParameter DT { get { return param("DT"); } }
		public ThemaItemParameter DT2 { get { return param("DT2"); } }
		public ThemaItemParameter X { get { return param("X"); } }
		public ThemaItemParameter S { get { return param("S"); } }
		public ThemaItemParameter U { get { return param("U"); } }
		public ThemaItemParameter condition { get { return param("condition"); } }


		public ParameterValue Av { get { return paramv("A"); } }
		public ParameterValue Bv { get { return paramv("B"); } }
		public ParameterValue Cv { get { return paramv("C"); } }
		public ParameterValue Dv { get { return paramv("D"); } }
		public ParameterValue Iv { get { return paramv("I"); } }
		public ParameterValue DTv { get { return paramv("DT"); } }
		public ParameterValue DT2v { get { return paramv("DT2"); } }
		public ParameterValue Xv { get { return paramv("X"); } }
		public ParameterValue Sv { get { return paramv("S"); } }
		public ParameterValue Uv { get { return paramv("U"); } }
		public ParameterValue conditionv { get { return paramv("condition"); } }

		[Test(Description = "проверяем уровень перекрытия между библиотекой и основной темой")]
		public void core_parameter_evidence_is_well()
		{
			Assert.AreEqual("l1.out",A.ContainingItem.Code);
			Assert.AreEqual("l1.out",B.ContainingItem.Code);
			Assert.AreEqual("A.out",C.ContainingItem.Code);
			Assert.AreEqual("l1.out",D.ContainingItem.Code);
			Assert.AreEqual("A.out",X.ContainingItem.Code);
			Assert.Null(condition.ContainingItem);
			Assert.True(condition.IsDynamic);
			Assert.True(condition.IsTarget);

		}

		[Test(Description = "проверяем уровень перекрытия между библиотекой и основной темой - с контекстом")]
		public void saved_and_user_parameter_evidence_is_well_context() {
			usrload();
			Assert.AreEqual("l1.out", A.ContainingItem.Code);
			Assert.AreEqual("l1.out", B.ContainingItem.Code);
			Assert.AreEqual("A.out", C.ContainingItem.Code);
			Assert.AreEqual("l1.out", D.ContainingItem.Code);
			Assert.AreEqual("A.out", X.ContainingItem.Code);
			Assert.Null(condition.ContainingItem);
			Assert.Null(S.ContainingItem);
			Assert.Null(U.ContainingItem);
			Assert.True(condition.IsDynamic);
			Assert.True(condition.IsTarget);
			Assert.True(S.IsDynamic);
			Assert.True(U.IsDynamic);
			Assert.True(S.IsSavedBased);
			Assert.True(U.IsUserBased);
		}

		private void usrload() {
			item = getitem("test\\admin", new WrapContext
			                              	{
			                              		SavedParameters = new Dictionary<string, string>
			                              		                  	{
			                              		                  		{"A","savedA"},
			                              		                  		{"B","savedB"},
																		{"S","savedOnly"}
			                              		                  	},
			                              		UserParameters = new Dictionary<string, string>{
			                              		                                               	{"A","userA"},{"X","userX"},{"U","useronly"}
			                              		                                               }
			                              	});
		}
	}
}