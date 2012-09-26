using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Comdiv.QWeb.Utils.LogicalExpressionLanguage;

namespace Comdiv.ThemaLoader.Wrap {
	public class ThemaItemWrapper : IThemaItemWrapper, ILogicTermSource {
		private IDictionary<string, string> _allcondvalues;
		private IList<IThemaItemElement> _allelements;
		private IDictionary<string, ThemaItemParameter> _allparameters;

		private IList<string> _conditions;
		private IDictionary<string, ParameterValue> _parametervalues;
		private LogicalExpressionEvaluator ceval;

		protected ThemaItemWrapper(IThemaItem item, IThemaWrapper wrapper) {
			Item = item;
			ThemaWrapper = wrapper;
			Context = wrapper.Context.GetChild();
		}

		#region ILogicTermSource Members

		bool ILogicTermSource.get(string name) {
			return ((ILogicTermSource) this).value(name).ToBool();
		}

		bool ILogicTermSource.equal(string name, string value) {
			return ((ILogicTermSource) this).value(name) == value;
		}

		string ILogicTermSource.value(string name) {
			((ILogicTermSource) this).getall();
			if (!_allcondvalues.ContainsKey(name)) return "";
			return _allcondvalues[name];
		}

		IDictionary<string, string> ILogicTermSource.getall() {
			if (_allcondvalues == null) {
				_allcondvalues = new Dictionary<string, string>();
				foreach (var parameter in Item.Thema.Parameters) {
					_allcondvalues[parameter.Key] = parameter.Value;
				}
				foreach (var parameter in Item.NativeXmlParameters) {
					_allcondvalues[parameter.Key] = parameter.Value;
				}
				foreach (var parameterValue in ParameterValues) {
					_allcondvalues[parameterValue.Key] = parameterValue.Value.ResolvedStringValue;
				}
				foreach (var condition in Conditions) {
					_allcondvalues[condition] = "true";
				}
			}
			return _allcondvalues;
		}

		#endregion

		#region IThemaItemWrapper Members

		public IDictionary<string, ThemaItemParameter> AllParameters {
			get {
				if (null == _allparameters) {
					_allparameters = new Dictionary<string, ThemaItemParameter>();
					foreach (var libraryLink in Item.LibraryLinks) {
						foreach (var parameter in libraryLink.Target.Parameters) {
							_allparameters[parameter.Code] = parameter;
						}
					}
					foreach (var parameter in Item.Parameters) {
						_allparameters[parameter.Code] = parameter;
					}

					foreach (var tp in _allparameters.Values.Where(x => x.Target.hasContent()).ToArray()) {
						if (!_allparameters.ContainsKey(tp.Target)) {
							_allparameters[tp.Target] = new ThemaItemParameter {Code = tp.Target, IsDynamic = true, IsTarget = true};
						}
					}

					if (Context.SavedParameters != null) {
						foreach (var sp in Context.SavedParameters) {
							if (!_allparameters.ContainsKey(sp.Key)) {
								_allparameters[sp.Key] = new ThemaItemParameter {Code = sp.Key, IsDynamic = true, IsSavedBased = true};
							}
						}
					}

					if (Context.UserParameters != null) {
						foreach (var sp in Context.UserParameters) {
							if (!_allparameters.ContainsKey(sp.Key)) {
								_allparameters[sp.Key] = new ThemaItemParameter {Code = sp.Key, IsDynamic = true, IsUserBased = true};
							}
						}
					}

					foreach (var unsecureparameter in _allparameters.Values.Where(x => !authorize(x)).ToArray()) {
						_allparameters.Remove(unsecureparameter.Code);
					}
				}
				return _allparameters;
			}
		}

		public IDictionary<string, ParameterValue> ParameterValues {
			get {
				if (null == _parametervalues) {
					//init
					_parametervalues = new Dictionary<string, ParameterValue>();
					foreach (var pdef in AllParameters.Values.ToArray()) {
						_parametervalues[pdef.Code] = new ParameterValue {Parameter = pdef, Parent = this};
					}
					//setup defaults
					foreach (var templateparameter in AllParameters.Values.Where(x => !x.IsDynamic).ToArray()) {
						_parametervalues[templateparameter.Code].OverrideValue(templateparameter.DefaultValue,
						                                                       ParameterResolveLevel.Definition);
					}
					//setup saved
					if (Context.SavedParameters != null) {
						foreach (var sp in Context.SavedParameters) {
							_parametervalues[sp.Key].OverrideValue(sp.Value, ParameterResolveLevel.Saved);
						}
					}
					//setup user
					if (Context.UserParameters != null) {
						foreach (var sp in Context.UserParameters) {
							_parametervalues[sp.Key].OverrideValue(sp.Value, ParameterResolveLevel.User);
						}
					}
					//makeup targets
					foreach (var targeted in AllParameters.Values.Where(x => x.Target.hasContent())) {
						_parametervalues[targeted.Target].AppendValue(_parametervalues[targeted.Code].StringValue);
					}
				}
				return _parametervalues;
			}
		}

		public IThemaWrapperFactory Factory {
			get { return ThemaWrapper.Factory; }
		}

		public IList<IThemaItemElement> GetAllElements() {
			if (null == _allelements) {
				_allelements = new List<IThemaItemElement>();
				foreach (var libraryLink in Item.LibraryLinks) {
					foreach (var element in libraryLink.Target.GetElements<IThemaItemElement>()) {
						addelement(element);
					}
				}
				foreach (var e in Item.GetElements<IThemaItemElement>()) {
					addelement(e);
				}
			}
			return _allelements;
		}

		public WrapContext Context { get; private set; }

		public IThemaWrapper ThemaWrapper { get; private set; }

		public IThemaItem Item { get; private set; }

		public void AccomodateContext() {
			redirectObject();
			redirectYear();
			redirectPeriod();

			checkObjectGroup();
			checkPeriod();
			checkYear();
		}


		public IList<string> Conditions {
			get {
				if (null == _conditions) {
					if (!ParameterValues.ContainsKey("condition")) {
						_conditions = new List<string>();
					}
					else {
						_conditions = ParameterValues["condition"].ResolvedStringValue.SmartSplit().Distinct().ToList();
					}
				}
				return _conditions;
			}
		}

		public bool EvalConditionString(string conditionformula) {
			ceval = ceval ?? (ceval = new LogicalExpressionEvaluator(this));
			return ceval.eval(conditionformula);
		}

		public string ResolveParametersInString(string value) {
			if (null == value) return "";
			if (!value.Contains("[[")) return value;
			return Regex.Replace(value, @"\[\[(?<name>[\s\S]+?)(\:(?<default>[\s\S]+?]))?\]\]",
			                     m =>
			                     	{
			                     		var name = m.Groups["name"].Value;
			                     		var def = m.Groups["default"].Value;
			                     		var result = ResolveName(name);
			                     		if (null == result) {
			                     			result = def ?? "";
			                     		}
			                     		return result;
			                     	},
			                     RegexOptions.Compiled);
		}

		public string ResolveName(string name) {
			if (ParameterValues.ContainsKey(name)) return ParameterValues[name].ResolvedStringValue;
			if (Item.NativeXmlParameters.ContainsKey(name)) return Item.NativeXmlParameters[name];
			if (Item.Thema.Parameters.ContainsKey(name)) return Item.Thema.Parameters[name];
			return null;
		}

		#endregion

		private bool authorize(ThemaItemParameter p) {
			if (p.IsDynamic) return true;
			if (p.Role.noContent() && p.ExRole.noContent()) return true;
			if (p.Role.hasContent()) {
				foreach (var inrole in p.Role.SmartSplit()) {
					if (!Factory.IsInRole(inrole)) return false;
				}
			}
			if (p.ExRole.hasContent()) {
				foreach (var exrole in p.ExRole.SmartSplit()) {
					if (Factory.IsInRole(exrole, true)) return false;
				}
			}
			return true;
		}

		public static IThemaItemWrapper Wrap(IThemaItem item, IThemaWrapper wrapper) {
			IThemaItemWrapper result = null;
			if (item is IReportThemaItem) {
				result = new ReportThemaItemWrapper(item, wrapper);
			}
			else if (item is IFormThemaItem) {
				result = new FormThemaItemWrapper(item, wrapper);
			}
			else {
				result = new ThemaItemWrapper(item, wrapper);
			}
			result.AccomodateContext();
			return result;
		}

		private void addelement(IThemaItemElement e) {
			var wrap = e.GetWrapper(this);
			if (wrap.IsValid()) {
				if (wrap.CustomCode.hasContent()) {
					var existed = _allelements.FirstOrDefault(x => x.CustomCode == wrap.CustomCode);
					if (null != existed) {
						_allelements.Remove(existed);
					}
				}
				_allelements.Add(wrap);
			}
		}

		private void redirectPeriod() {
			var rules =
				Item.ListPeriodRedirect.Where(x => x.Source == Context.Period).OrderByDescending(x => x.ForGroup).ToArray();
			if (0 == rules.Length) return;
			foreach (var r in rules) {
				if (r.ForGroup.hasContent()) {
					if (Context.ObjectGroups.hasContent()) {
						if (Context.ObjectGroups.SmartSplit(false, true, '/', ';').Contains(r.ForGroup)) {
							Context.Period = r.Target;
							Context.PeriodRedirected = true;
							break;
						}
					}
				}
				else {
					Context.Period = r.Target;
					Context.PeriodRedirected = true;
					break;
				}
			}
		}

		private void redirectYear() {
			//NOTE: logic for this is not now needed so will be implemented on needs
		}

		private void redirectObject() {
			if (null == Context.TargetObject && 0 == Context.ObjectId) return;
			//NOTE: only fixed-object now is applyable to redirect
			var fixedobject = Item.FixedObject.hasContent() ? Item.FixedObject : Item.Thema.FixedObject;
			if (fixedobject.hasContent()) {
				//TODO: нужен механизм правильного проброса типа объекта
				if (null == Context.TargetObject) {
					throw new Exception("for now fixed object can be applyed only if certain other object is in context");
				}
				if (null == entityResolver()) {
					throw new Exception("cannot redirect target without entity resolver");
				}
				Context.TargetObject = entityResolver().Get(Context.TargetObject.GetType(), fixedobject);
				Context.TryGetGroup();
				Context.ObjectRedirected = true;
			}
		}

		protected IEntityResolver entityResolver() {
			return Factory.Factory.EntityResolver;
		}

		private void checkYear() {
			if (0 == Context.Year) return;
			if (Item.Thema.UseHistory) return;
			if (Context.Year < Item.Thema.FirstYear || Context.Year > Item.Thema.LastYear) {
				Context.YearIsValid = false;
			}
		}

		private void checkPeriod() {
			if (0 == Context.Period) return;
			if (0 == Item.ListForPeriods.Length) return;
			if (-1 == Array.IndexOf(Item.ListForPeriods, Context.Period)) {
				Context.PeriodIsValid = false;
			}
		}

		private void checkObjectGroup() {
			var requestedgroups = Item.Thema.ListForGroup;
			if (0 == requestedgroups.Length) return;
			var actualgroups = Context.ObjectGroups.SmartSplit(false, true, '/', ';');
			if (0 == actualgroups.Intersect(requestedgroups).Count()) {
				Context.GroupIsValid = false;
			}
		}
	}
}