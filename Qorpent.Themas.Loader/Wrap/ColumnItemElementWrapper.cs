using System;
using System.Linq;
using Comdiv.ThemaLoader.ZetaIntegration;

namespace Comdiv.ThemaLoader.Wrap {
	public class ColumnItemElementWrapper : ThemaItemElementWrapper, IColumnItemElementWrapper {
		private string _forgroup;
		private string _forperiods;
		private string _matrixformula;
		private string _matrixtotalformula;
		private int[] _periods;
		private string _title;
		private string _valuta;
		private int _year;

		public ColumnItemElementWrapper(IColumnItemElement target, IThemaItemWrapper item) : base(target, item) {
			zetaObjectType = typeof (IZetaColumnIntermediate);
		}

		protected IColumnItemElement MyColumn {
			get { return (IColumnItemElement) Target; }
		}

		protected IZetaColumnIntermediate MyZetaColumn {
			get { return ZetaObject as IZetaColumnIntermediate; }
		}

		public string Title {
			get { return _title ?? (_title = prepareTitle(Name)); }
			set { _title = value; }
		}

		#region IColumnItemElementWrapper Members

		public string ForPeriods {
			get { return _forperiods ?? (_forperiods = prepared(MyColumn.ForPeriods)); }
			set { _forperiods = value; }
		}

		public override bool IsValid() {
			var reuslt = base.IsValid();
			if (!reuslt) return false;
			if (!checkForPeriod()) return false;
			if (!checkForGroup()) return false;
			return true;
		}

		public string ForGroup {
			get { return _forgroup ?? (_forgroup = prepared(MyColumn.ForGroup)); }
			set { _forgroup = value; }
		}

		public string Valuta {
			get {
				if (null != ZetaObject) {
					return ZetaObject.Valuta;
				}
				return _valuta ?? (_valuta = prepared(MyColumn.Valuta));
			}
			set {
				if (null != ZetaObject) {
					throw new Exception("cannot change value due to DB Aware");
				}
				_valuta = value;
			}
		}

		public string MatrixId {
			get { return MyColumn.MatrixId; }
			set { throw new NotImplementedException(); }
		}

		public string MatrixFormula {
			get { return _matrixformula ?? (_matrixformula = prepared(MyColumn.MatrixFormula)); }
			set { _matrixformula = value; }
		}

		public string MatrixFormulaType {
			get { return MyColumn.MatrixFormulaType; }
			set { throw new NotImplementedException(); }
		}

		public string MatrixTotalFormula {
			get { return _matrixtotalformula ?? (_matrixtotalformula = prepared(MyColumn.MatrixTotalFormula)); }
			set { _matrixtotalformula = value; }
		}

		public bool Fixed {
			get { return MyColumn.Fixed; }
			set { throw new NotImplementedException(); }
		}

		public int Year {
			get {
				if (0 == _year) {
					prepareYearAndPeriod();
				}
				return _year;
			}
			set { _year = value; }
		}

		public int Period {
			get {
				if (null == _periods) prepareYearAndPeriod();
				return _periods.FirstOrDefault();
			}
			set { throw new NotImplementedException(); }
		}

		public int[] Periods {
			get {
				if (null == _periods) {
					prepareYearAndPeriod();
				}
				return _periods;
			}
			set { _periods = value; }
		}


		public bool DoSum {
			get {
				if (null != ZetaObject) {
					return ZetaObject.Marks.Contains("/DOSUM/");
				}
				return MyColumn.DoSum;
			}
			set { throw new NotImplementedException(); }
		}

		#endregion

		private bool checkForGroup() {
			if (ForGroup.noContent()) return true;
			if (ItemWrap.Context.ObjectId == 0 && ItemWrap.Context.ObjectGroups.noContent()) {
				return true;
			}
			var mygroups = ForGroup.SmartSplit();
			var contxtgroups = ItemWrap.Context.ObjectGroups.SmartSplit();
			return null != mygroups.Intersect(contxtgroups).FirstOrDefault();
		}

		private bool checkForPeriod() {
			if (ForPeriods.noContent()) return true;
			if (0 == ItemWrap.Context.Period) return true;
			foreach (var i in ForPeriods.SmartSplit().Select(x => x.ToInt())) {
				if (i == ItemWrap.Context.Period) return true;
			}
			return false;
		}

		protected override bool checkGroup() {
			var result = base.checkGroup();
			if (result) return true;
			var colgroup = ItemWrap.ResolveName("colgroup");
			if (colgroup.noContent() && (Group.noContent() || Group == "default")) return true;
			foreach (var grp in colgroup.SmartSplit()) {
				if (Group == grp) return true;
			}
			return false;
		}

		protected string prepareTitle(string name) {
			if (!name.Contains("{")) return name;
			var fstperiod = Periods.FirstOrDefault();
			var periodname = fstperiod.ToString();
			if (ContainingItem.Thema.Factory.PeriodProvider != null) {
				periodname = ContainingItem.Thema.Factory.PeriodProvider.GetName(fstperiod);
			}
			return string.Format(name, Year, fstperiod, periodname);
		}

		private void prepareYearAndPeriod() {
			var year = MyColumn.Year;
			var period = MyColumn.Period;
			if (period >= 0) {
				//not formula period - we can proceed such resolution without IPeriodProvider
				if (year > 1900) {
					_year = year;
				}
				else {
					_year = ItemWrap.Context.Year + year;
				}
				if (0 == period) {
					_periods = new[] {ItemWrap.Context.Period};
				}
				else {
					_periods = new[] {period};
				}
			}
			else {
				if (null == ContainingItem.Thema.Factory.PeriodProvider) {
					throw new ThemaLoaderException("current factory not supports PeriodProvider, so formula periods not allowed " +
					                               XmlSource.Describe().ToWhereString());
				}
				var def = ContainingItem.Thema.Factory.PeriodProvider.Eval(
					ItemWrap.Context.Year, ItemWrap.Context.Period, year, period
					);
				_year = def.Year;
				_periods = def.Periods;
			}
		}
	}
}