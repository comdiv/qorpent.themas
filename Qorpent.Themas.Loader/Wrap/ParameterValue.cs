namespace Comdiv.ThemaLoader.Wrap {
	public class ParameterValue {
		private string _resolved;
		private object _value;
		public ThemaItemParameter Parameter { get; set; }
		public IThemaItemWrapper Parent { get; set; }
		public string StringValue { get; set; }

		public string ResolvedStringValue {
			get {
				if (_resolved == null) {
					_resolved = Parent.ResolveParametersInString(StringValue);
				}
				return _resolved;
			}
		}


		public ParameterResolveLevel ResolveLevel { get; set; }

		public object Value {
			get { return _value ?? (_value = ResolvedStringValue.to(Parameter.ResolvedType)); }
		}

		public ParameterValue OverrideValue(string value, ParameterResolveLevel level) {
			if (value != StringValue) {
				StringValue = value;
				ResolveLevel = level;
			}
			return this;
		}

		public ParameterValue AppendValue(string value) {
			ResolveLevel = ParameterResolveLevel.Target;
			StringValue += ";" + value;
			return this;
		}
	}
}