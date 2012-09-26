using Qorpent.Mvc;

namespace Qorpent.Themas.Compiler.QWeb {
	[Action("thema.compilesingle")]
	public class CompileSingleAction : ActionBase {
		public ThemaCompilerContext LastContext;
		[Bind] public string Text;
		[Bind] public string ProjBxl;
		protected override object process() {
			var project = new SingleContentProject(Text);
			if(ProjBxl.hasContent()) {
				project.ConfigureFromXml(MyBxl.Parse(ProjBxl));
			}
			return new ThemaCompilerResultForQweb(
				LastContext = new ThemaCompiler().Compile(
					project));
		}
	}
}