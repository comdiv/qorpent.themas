using System.Collections.Generic;

namespace Comdiv.ThemaLoader.Wrap {
	public class WrapContext {
		private object _targetObject;

		public WrapContext() {
			YearIsValid = true;
			PeriodIsValid = true;
			GroupIsValid = true;
		}

		public WrapContext MasterContext { get; set; }

		public object TargetObject {
			get { return _targetObject; }
			set {
				if (_targetObject != value) {
					_targetObject = value;
					TryGetGroup();
				}
			}
		}

		public int ObjectId { get; set; }
		public string ObjectGroups { get; set; }
		public int Year { get; set; }
		public int Period { get; set; }
		public bool GroupIsValid { get; set; }
		public bool YearIsValid { get; set; }
		public bool PeriodIsValid { get; set; }
		public bool ObjectRedirected { get; set; }
		public bool YearRedirected { get; set; }
		public bool PeriodRedirected { get; set; }
		public IDictionary<string, string> SavedParameters { get; set; }
		public IDictionary<string, string> UserParameters { get; set; }

		public bool IsValid {
			get { return GroupIsValid && YearIsValid && PeriodIsValid; }
		}

		public bool IsChanged {
			get { return ObjectRedirected || YearRedirected || PeriodRedirected; }
		}

		public void TryGetGroup() {
			if (MasterContext != null && TargetObject == MasterContext.TargetObject) {
				ObjectGroups = MasterContext.ObjectGroups;
				ObjectId = MasterContext.ObjectId;
				return;
			}
			ObjectGroups = "";
			ObjectId = 0;
			if (null == TargetObject) return;
			if (TargetObject is IGroupListContainer) {
				ObjectGroups = ((IGroupListContainer) TargetObject).GroupCache;
				ObjectId = ((IGroupListContainer) TargetObject).Id;
			}
		}

		public WrapContext GetChild() {
			var result = MemberwiseClone() as WrapContext;
			result.MasterContext = this;
			return result;
		}
	}
}