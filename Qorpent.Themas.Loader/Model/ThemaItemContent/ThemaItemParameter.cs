using System;
using System.Xml.Linq;

namespace Comdiv.ThemaLoader {
	public class ThemaItemParameter {
		private Type _type;

		public ThemaItemParameter() {
			Type = "string";
		}

		public string Tab { get; set; }
		public string Group { get; set; }
		public string Role { get; set; }
		public string ExRole { get; set; }
		public string Target { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }
		public string Type { get; set; }
		public string DefaultValue { get; set; }
		public string ListDefinition { get; set; }
		public string CustomView { get; set; }
		public bool IsRadio { get; set; }
		public bool Visible { get; set; }
		public bool Readonly { get; set; }


		public ThemaItem ContainingItem { get; set; }
		public XElement XmlSource { get; set; }

		public Type ResolvedType {
			get {
				if (null == _type) {
					lock (this) {
						switch (Type) {
							case "":
								goto case "string";
							case null:
								goto case "string";
							case "string":
								_type = (typeof (string));
								break;
							case "int":
								_type = typeof (int);
								break;
							case "date":
								_type = typeof (DateTime);
								break;
							case "bool":
								_type = typeof (bool);
								break;
							default:
								_type = System.Type.GetType(Type);
								break;
						}
					}
				}
				return _type;
			}
		}

		public bool IsTarget { get; set; }

		public bool IsDynamic { get; set; }

		public bool IsSavedBased { get; set; }

		public bool IsUserBased { get; set; }
	}
}