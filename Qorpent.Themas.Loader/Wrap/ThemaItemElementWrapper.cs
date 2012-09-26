using System;
using System.Xml.Linq;
using Comdiv.ThemaLoader.ZetaIntegration;

namespace Comdiv.ThemaLoader.Wrap {
	public class ThemaItemElementWrapper : IThemaItemElementWrapper {
		private bool? _authorized;
		private string _code;
		private string _condition;
		private string _cssclass;
		private string _cssstyle;
		private string _customcode;
		private string _customview;
		private string _formula;
		private string _name;
		private string _numberformat;
		private string _tag;
		private bool _zetaobjchecked;
		private IZetaEntityIntermediate _zetaobject;
		protected Type zetaObjectType;

		public ThemaItemElementWrapper(IThemaItemElement target, IThemaItemWrapper item) {
			Target = target;
			ItemWrap = item;
		}

		#region IThemaItemElementWrapper Members

		public IThemaItemWrapper ItemWrap { get; set; }
		public IThemaItemElement Target { get; set; }


		public string Role {
			get { return Target.Role; }
			set { throw new NotSupportedException(); }
		}

		public string Code {
			get { return _code ?? (_code = prepared(Target.Code)); }
			set { _code = value; }
		}

		public string Name {
			get {
				if (_name.noContent()) {
					_name = prepared(Target.Name);
					if (_name.noContent() && null != ZetaObject) {
						_name = ZetaObject.Name;
					}
				}
				return _name;
			}
			set { _name = value; }
		}

		public XElement XmlSource {
			get { return Target.XmlSource; }
			set { throw new NotSupportedException(); }
		}

		public bool Authorized(string usr) {
			if (!_authorized.HasValue) {
				_authorized = false;
				var roles = Role.SmartSplit();
				foreach (var role in roles) {
					if (Factory.IsInRole(role)) {
						_authorized = true;
						break;
					}
				}
			}
			return _authorized.Value;
		}

		public string Group {
			get { return Target.Group; }
			set { throw new NotSupportedException(); }
		}

		public IZetaEntityIntermediate ZetaObject {
			get {
				if (!_zetaobjchecked && null == _zetaobject) {
					if (null != ContainingItem.ZetaObject) {
						_zetaobject = ContainingItem.ZetaObject;
					}
					else if (null != zetaObjectType && null != Factory.Factory.EntityResolver) {
						_zetaobject = (IZetaEntityIntermediate) Factory.Factory.EntityResolver.Get(zetaObjectType, Code);
					}
				}
				_zetaobjchecked = true;
				return _zetaobject;
			}
		}

		public string CustomCode {
			get { return _customcode ?? (_customcode = prepared(Target.CustomCode)); }
			set { _customcode = value; }
		}

		public string Condition {
			get { return _condition ?? (_condition = prepared(Target.Condition)); }
			set { _condition = value; }
		}

		public string Comment {
			get {
				if (Target.Comment.hasContent()) {
					return Target.Comment;
				}
				if (null != ZetaObject) {
					return ZetaObject.Comment;
				}
				return "";
			}
			set { throw new NotSupportedException(); }
		}

		public string Evidence {
			get { return Target.Evidence; }
			set { throw new NotSupportedException(); }
		}

		public IThemaItem ContainingItem {
			get { return Target.ContainingItem; }
			set { throw new NotSupportedException(); }
		}

		public string Type {
			get { return Target.Type; }
			set { throw new NotSupportedException(); }
		}

		public bool IsFormula {
			get {
				if (null != ZetaObject) {
					return ZetaObject.IsFormula;
				}
				return Target.IsFormula;
			}
			set { throw new NotSupportedException(); }
		}

		public string Formula {
			get {
				if (null != ZetaObject) {
					return ZetaObject.Formula;
				}
				return _formula ?? (_formula = prepared(Target.Formula));
			}
			set {
				if (null != ZetaObject) {
					throw new Exception("formula is immutable due to DB aware");
				}
				_formula = value;
			}
		}

		public string FormulaType {
			get {
				if (null != ZetaObject) {
					return ZetaObject.FormulaType;
				}
				return Target.FormulaType;
			}
			set { throw new NotSupportedException(); }
		}

		public string CssClass {
			get { return _cssclass ?? (_cssclass = prepared(Target.CssClass)); }
			set { _cssclass = value; }
		}

		public string CssStyle {
			get { return _cssstyle ?? (_cssstyle = prepared(Target.CssStyle)); }
			set { _cssstyle = value; }
		}

		public string CustomView {
			get { return _customview ?? (_customview = prepared(Target.CustomView)); }
			set { _customview = value; }
		}

		public string Tag {
			get {
				if (null != ZetaObject) {
					return ZetaObject.Tag;
				}
				return _tag ?? (_tag = prepared(Target.Tag));
			}
			set {
				if (null != ZetaObject) {
					throw new Exception("tag is immutable due to DB aware");
				}
				_tag = value;
			}
		}

		public bool Immutable {
			get { return Target.Immutable; }
			set { throw new NotSupportedException(); }
		}

		public string NumberFormat {
			get { return _numberformat ?? (_numberformat = prepared(Target.NumberFormat)); }
			set { _numberformat = value; }
		}

		public IThemaItemElementWrapper GetWrapper(IThemaItemWrapper itemwrapper) {
			if (itemwrapper == ItemWrap) return this;
			return Target.GetWrapper(itemwrapper);
		}

		public IThemaWrapperFactory Factory {
			get { return ItemWrap.Factory; }
		}

		public virtual bool IsValid() {
			if (!checkRole()) return false;
			if (!checkGroup()) return false;
			if (!checkCondition()) return false;
			return true;
		}

		#endregion

		protected string prepared(string value) {
			return ItemWrap.ResolveParametersInString(value);
		}

		private bool checkCondition() {
			if (Condition.noContent()) return true;
			return ItemWrap.EvalConditionString(Condition);
		}

		protected virtual bool checkGroup() {
			var elementgroup = ItemWrap.ResolveName("elementgroup");
			if (elementgroup.noContent() && (Group.noContent() || Group == "default")) return true;
			foreach (var grp in elementgroup.SmartSplit()) {
				if (Group == grp) return true;
			}
			return false;
		}

		private bool checkRole() {
			if (Role.noContent()) return true;
			foreach (var role in Role.SmartSplit()) {
				if (Factory.IsInRole(role)) return true;
			}
			return false;
		}
	}
}