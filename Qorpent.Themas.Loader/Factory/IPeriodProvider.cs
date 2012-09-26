namespace Comdiv.ThemaLoader {
	public interface IPeriodProvider {
		string GetName(int id);
		YearPeriodRecord Eval(int contextyear, int contextperiod, int targetyear, int targetperiod);
	}
}