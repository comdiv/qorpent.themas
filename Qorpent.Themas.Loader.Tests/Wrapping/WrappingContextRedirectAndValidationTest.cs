using Comdiv.ThemaLoader.Wrap;
using NUnit.Framework;

namespace Comdiv.ThemaLoader.Test.Wrapping {
	[TestFixture]
	public class WrappingContextRedirectAndValidationTest:WTBaseTest {
		public WrappingContextRedirectAndValidationTest() {
			this.code = @"
thema X1 
	forgroup=G1
	in A1.in
thema X2
	forgroup=G2		
	in A2.in
thema X3
	forgroup=KVART
	in A3.in
thema X4
	fixedobject=2
	usehistory=true
	firstyear=2011
	in A4.in
		forperiods=11;12;13
thema X5
	firstyear=2011
	in A5.in
		forperiods=11;12;13
		periodredirect=':1=11;2=12|KVART:1=13;301=11'
";
			
		}

		[Test]
		public void year_checked() {
			var ent = new WTEntityResolver();
			var obj2 = ent.Get<WTObject>(2);
			var ctx = new WrapContext { TargetObject = obj2, Period = 11, Year = 2010};
			var wf = load("test\\admin");
			Assert.True(wf.WrapForm("X4.A4",ctx).Context.IsValid);
			Assert.False(wf.WrapForm("X5.A5", ctx).Context.IsValid);
			Assert.False(wf.WrapForm("X5.A5", ctx).Context.YearIsValid);
		}

		[Test]
		public void period_redirect_must_be_applyed()
		{
			var ent = new WTEntityResolver();
			var obj1 = ent.Get<WTObject>(1);
			var obj2 = ent.Get<WTObject>(2); //KVART
			var ctx1 = new WrapContext { TargetObject = obj1, Period = 1 };
			var ctx2 = new WrapContext { TargetObject = obj2, Period = 1 };
			var wf = load("test\\admin");
			var v1 = wf.WrapForm("X5.A5", ctx1).Context;
			var v2 = wf.WrapForm("X5.A5", ctx2).Context;
			Assert.True(v1.IsValid);
			Assert.True(v1.IsChanged);
			Assert.True(v1.PeriodRedirected);
			Assert.AreEqual(11,v1.Period);

			Assert.True(v2.IsValid);
			Assert.True(v2.IsChanged);
			Assert.True(v2.PeriodRedirected);
			Assert.AreEqual(13, v2.Period);
			

		}

		[Test]
		public void forperiod_must_match() {
			var ent = new WTEntityResolver();
			var obj1 = ent.Get<WTObject>(1);
			var ctx1 = new WrapContext { TargetObject = obj1, Period = 14 };
			var ctx2 = new WrapContext { TargetObject = obj1, Period = 11 };
			var ctx3 = new WrapContext { TargetObject = obj1, Period = 1 };
			var wf = load("test\\admin");
			Assert.False(wf.WrapForm("X4.A4", ctx1).Context.IsValid);
			Assert.True(wf.WrapForm("X4.A4", ctx2).Context.IsValid);
			Assert.False(wf.WrapForm("X4.A4", ctx3).Context.IsValid);

			Assert.False(wf.WrapForm("X5.A5", ctx1).Context.IsValid);
			Assert.True(wf.WrapForm("X5.A5", ctx2).Context.IsValid);
			Assert.True(wf.WrapForm("X5.A5", ctx3).Context.IsValid);
			
		}

		[Test]
		public void object_redirected_with_fixedobject() {
			var ent = new WTEntityResolver();
			var obj1 = ent.Get<WTObject>(1);
			var obj2 = ent.Get<WTObject>(2);
			var obj3 = ent.Get<WTObject>(3);
			var wf = load("test\\admin");
			var ctx = new WrapContext { TargetObject = obj1, Period = 11 };
			var v1 = wf.WrapForm("X4.A4.in", ctx);
			ctx = new WrapContext { TargetObject = obj2, Period = 11 };
			var v2 = wf.WrapForm("X4.A4.in", ctx);
			ctx = new WrapContext { TargetObject = obj3, Period = 11 };
			var v3 = wf.WrapForm("X4.A4.in", ctx);
			Assert.True(v1.Context.ObjectRedirected == v2.Context.ObjectRedirected == v3.Context.ObjectRedirected);
			Assert.True((v1.Context.ObjectId == v2.Context.ObjectId) && (v2.Context.ObjectId == v3.Context.ObjectId));
			Assert.True((v1.Context.ObjectGroups == v2.Context.ObjectGroups) && (v2.Context.ObjectGroups == v3.Context.ObjectGroups));
			Assert.True(v1.Context.ObjectRedirected);
			Assert.AreEqual(2,v1.Context.ObjectId);
			Assert.AreEqual("/G1/KVART/",v1.Context.ObjectGroups);
			Assert.True(v1.Context.IsChanged);
			Assert.True(v1.Context.IsValid);
		}

		[Test]
		public void forgroup_checked_well() {
			var ent = new WTEntityResolver();
			var obj1 = ent.Get<WTObject>(1);
			var obj2 = ent.Get<WTObject>(2);
			var obj3 = ent.Get<WTObject>(3);
			var wf = load("test\\admin");
			var ctx = new WrapContext {TargetObject = obj1, Period=11};
			Assert.True(wf.WrapForm("X1.A1",ctx).Context.IsValid);
			Assert.True(wf.WrapForm("X2.A2", ctx).Context.IsValid);
			Assert.False(wf.WrapForm("X3.A3", ctx).Context.IsValid);
			Assert.True(wf.WrapForm("X4.A4", ctx).Context.IsValid);
			Assert.True(wf.WrapForm("X5.A5", ctx).Context.IsValid);

			ctx = new WrapContext { TargetObject = obj2, Period = 11 };
			Assert.True(wf.WrapForm("X1.A1", ctx).Context.IsValid);
			Assert.False(wf.WrapForm("X2.A2", ctx).Context.IsValid);
			Assert.True(wf.WrapForm("X3.A3", ctx).Context.IsValid);
			Assert.True(wf.WrapForm("X4.A4", ctx).Context.IsValid);
			Assert.True(wf.WrapForm("X5.A5", ctx).Context.IsValid);

			ctx = new WrapContext { TargetObject = obj3, Period = 11 };
			Assert.True(wf.WrapForm("X1.A1", ctx).Context.IsValid);
			Assert.False(wf.WrapForm("X2.A2", ctx).Context.IsValid);
			Assert.False(wf.WrapForm("X3.A3", ctx).Context.IsValid);
			Assert.True(wf.WrapForm("X4.A4", ctx).Context.IsValid);
			Assert.True(wf.WrapForm("X5.A5", ctx).Context.IsValid);
		}
	}
}