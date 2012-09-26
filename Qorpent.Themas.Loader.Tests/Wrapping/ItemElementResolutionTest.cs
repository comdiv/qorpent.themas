using System;
using System.Collections.Generic;
using System.Linq;
using Comdiv.ThemaLoader.Wrap;
using NUnit.Framework;

namespace Comdiv.ThemaLoader.Test.Wrapping {
	[TestFixture]
	public class ItemElementResolutionTest : WTBaseTest {
		private IThemaItemWrapper item;
		private IList<IThemaItemElement> elements;

		public ItemElementResolutionTest()
		{
			this.code = @"
thema lib, islibrary=true
	out l1.out
		col Б1
		col Б1, customcode=lastyear, year = -1, period = 4
		col ZB1 
		col ZF1 
		col FORADM, role=ADMIN
		
thema X, myname
	tattr = 3
	activeA=true
	out A.out, iattr = 2
		uselib lib.l1.out
		col Б2, customcode=lastyear, year = -1, period = 4
		col C1, condition='test1 & test2'
		col FORPER, forperiods=15
";
		}
		[SetUp]
		public void setup() {
			this.item = getitem();
			
		}

		IThemaItemWrapper getitem(string usr = "test\\admin",WrapContext context = null) {
			context = context ?? new WrapContext {ObjectId = 1, ObjectGroups = "/G1/KVART/", Year = 2012, Period = 1};
			var result = load(usr).WrapReport("X.A",context);
			this.elements = result.GetAllElements();
			return result;
		}

		[Test]
		public void non_valid_period_for_forperiods() {
			getitem();
			Assert.Null(elements.FirstOrDefault(x=>x.Code=="FORPER"));
		}
		[Test]
		public void valid_period_for_forperiods()
		{
			getitem("test\\admin",new WrapContext{Period = 15});
			Assert.NotNull(elements.FirstOrDefault(x => x.Code == "FORPER"));
		}

		[Test]
		public void admin_column_allowed_for_admin() {
			getitem("test\\admin");
			Assert.NotNull(elements.First(x=>x.Code=="FORADM"));
		}
		[Test]
		public void admin_column_not_allowed_for_non_admin()
		{
			getitem("test\\usr");
			Assert.Null(elements.FirstOrDefault(x => x.Code == "FORADM"));
		}

		[Test]
		public void conditions_column_not_used_without_conditions() {
			var e = elements.FirstOrDefault(x => x.Code == "C1") as IColumnItemElementWrapper;
			Assert.Null(e);
		}

		[Test]
		public void conditions_column_used_with_conditions() {
			item = getitem("test\\admin",
			        new WrapContext {UserParameters = new Dictionary<string, string> {{"condition", "test1,test2"}}});
			var e = elements.FirstOrDefault(x => x.Code == "C1") as IColumnItemElementWrapper;
			Assert.NotNull(e);
		}


		[Test]
		public void zeta_integration_used() {
			var zb1 = elements.First(x=>x.Code=="ZB1") as IColumnItemElementWrapper;
			var zf1 = elements.First(x => x.Code == "ZF1") as IColumnItemElementWrapper;
			var e1 = elements.First(x => x.Code == "Б1") as IColumnItemElementWrapper;
			Assert.NotNull(zb1);
			Assert.NotNull(zf1);
			Assert.NotNull(zb1.ZetaObject);
			Assert.NotNull(zf1.ZetaObject);
			Assert.Null(e1.ZetaObject);
			Assert.AreEqual("Z B 1",zb1.Name);
			Assert.AreEqual("Z F 1", zf1.Name);
			Assert.True(zf1.IsFormula);
			Assert.AreEqual("@ZB1?",zf1.Formula);
			Assert.AreEqual("boo",zf1.FormulaType);
			e1.Formula = "test"; //must allow
			Exception ex = null;
			ex = Assert.Throws<Exception>(() => zb1.Formula = "test");
			StringAssert.Contains("db aware",ex.Message.ToLower());
		}

		[Test]
		public void simple_load_test() {

			// проверят загрузку 2-х первых колонок с контроллем перекрытия и преобразования года
			var e1 = elements.First(x=>x.Code=="Б1") as IColumnItemElementWrapper;
			var e2 = elements.First(x=>x.CustomCode=="lastyear") as IColumnItemElementWrapper;
			Assert.IsInstanceOf<IColumnItemElementWrapper>(e1);
			Assert.IsInstanceOf<IColumnItemElementWrapper>(e2);
			Assert.AreEqual(2012,e1.Year);
			Assert.AreEqual(2011,e2.Year);
			Assert.AreEqual(1,e1.Period);
			Assert.AreEqual(4,e2.Period);
			Assert.AreEqual("A.out",e2.ContainingItem.Code);
			Assert.AreEqual("l1.out",e1.ContainingItem.Code);
		}
	}
}