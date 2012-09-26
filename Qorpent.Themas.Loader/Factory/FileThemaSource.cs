using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Comdiv.QWeb.Files;
using Comdiv.QWeb.Serialization.BxlParser;

namespace Comdiv.ThemaLoader {
	public class FileThemaSource : IThemaSource {
		public FileThemaSource(params string[] paths) {
			Paths = paths ?? new string[] {};
		}

		public string[] Paths { get; set; }

		#region IThemaSource Members

		public IEnumerable<XElement> GetXmlSources(IThemaFactory factory) {
			var myfiles = new List<string>();
			foreach (var src in Paths) {
				//file mask
				var ext = Path.GetExtension(src);
				if (src.Contains("*")) {
					var dir = Path.GetDirectoryName(src);

					var mask = Path.GetFileName(src);
					var files =
						factory.FileResolver
							.ResolveAll(null, FileResolveResultType.FullPath, new[] {dir}, new[] {mask}, null)
							.OrderBy(x => x);
					foreach (var file in files) {
						myfiles.Add(file);
					}
				}
				else if (ext.Length > 0 && ext.Length <= 5) {
					// direct file
					var file = factory.FileResolver.Resolve(src);
					myfiles.Add(file);
				}
				else {
					//folder
					var dir = factory.FileResolver.Resolve(src);
					var files = Directory.GetFiles(dir, "*.thema.xml").OrderBy(x => x);
					foreach (var file in files) {
						myfiles.Add(file);
					}
				}
			}
			foreach (var myfile in myfiles) {
				if (Path.GetExtension(myfile) == ".xml") {
					yield return XElement.Load(myfile);
				}
				else if (Path.GetExtension(myfile) == ".bxl") {
					yield return new BxlXmlParser().Parse(File.ReadAllText(myfile), myfile);
				}
			}
		}

		#endregion
	}
}