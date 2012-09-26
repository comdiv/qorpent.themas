using System.Linq;

namespace Qorpent.Themas.Compiler.QWeb {
	[Serialize]
	public class ThemaCompilerResultForQweb {
		public ThemaCompilerResultForQweb(ThemaCompilerContext context) {
			IsComplete = context.IsComplete;
			Errors = context.Errors.ToArray();
			if (!IsComplete) return;
			var r = new XElement("result");
			foreach (var t in context.Themas.Values.Where(t => null != t.Xml)) {
				r.Add(t.Xml);
			}
			if (null != context.ExtraData) {
				r.Add(context.ExtraData);
			}
			Result = new XmlToBxlConverter().Convert(r);
			Log = context.Project.GetLog();
		}

		[Serialize] public string Log { get; set; }

		[Serialize] public string Result { get; set; }

		[Serialize] public ThemaCompilerError[] Errors { get; set; }

		[Serialize] public bool IsComplete { get; set; }
	}
}