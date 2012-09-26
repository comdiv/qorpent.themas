using System.Collections.Generic;

namespace Comdiv.ThemaLoader.Wrap {
	public interface IThemaItemWrapper {
		IThemaItem Item { get; }
		WrapContext Context { get; }
		IThemaWrapper ThemaWrapper { get; }
		IList<string> Conditions { get; }
		IDictionary<string, ThemaItemParameter> AllParameters { get; }
		IDictionary<string, ParameterValue> ParameterValues { get; }
		IThemaWrapperFactory Factory { get; }
		void AccomodateContext();
		string ResolveParametersInString(string value);
		bool EvalConditionString(string conditionformula);
		string ResolveName(string name);
		IList<IThemaItemElement> GetAllElements();
	}
}