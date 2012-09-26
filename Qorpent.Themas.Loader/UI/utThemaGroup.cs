namespace Comdiv.ThemaLoader.UI {
	public class utThemaGroup {
		public string Idx { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }
		public IThema Target { get; set; }
		public utThemaRoot[] Roots { get; set; }

		public UserThemaTree Tree { get; set; }
	}
}