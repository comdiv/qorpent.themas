using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Comdiv.QWeb.Logging;

namespace Comdiv.ThemaLoader {
	public class Loader : IThemaLoader {
		private readonly ILogListener log;

		private readonly IDictionary<string, ILoadXmlGenerator> _xmlloadgenerators =
			new Dictionary<string, ILoadXmlGenerator>();

		private XElement _extradata;
		private List<XElement> _xmlsources;

		public Loader(IThemaFactory factory) {
			Factory = factory;
			log = factory.Log;
		}

		private IEnumerable<IThemaItem> allitems {
			get { return Factory.Themas.Index.Values.SelectMany(x => x.Items); }
		}

		private IEnumerable<IThema> allthemas {
			get { return Factory.Themas.Index.Values; }
		}

		#region IThemaLoader Members

		public IThemaFactory Factory { get; set; }

		public IThemaLoader Load(params IThemaSource[] sources) {
			log.debug("Thema Loader -> start");
			init();
			if (sources != null && sources.Length > 0) {
				Factory.Sources.Clear();
				foreach (var themaSource in sources) {
					Factory.Sources.Add(themaSource);
				}
			}
			loadSources();
			extractLoadTimeGenerators();
			applyXmlLoadGenerators();
			extractElements();
			prepareCoreIndex();
			resolveLinks();
			resolveLibraries();
			resolveDepends();
			log.debug("Thema Loader -> finish");
			return this;
		}

		public List<XElement> XmlSources {
			get { return _xmlsources; }
		}

		public List<XElement> ThemaElements { get; private set; }

		#endregion

		private void applyXmlLoadGenerators() {
			foreach (var e in _xmlsources) {
				foreach (var gc in e.Descendants("call").ToArray()) {
					var code = gc.Id();
					if (_xmlloadgenerators.ContainsKey(code)) {
						gc.ReplaceWith(_xmlloadgenerators[code].Generate(gc, this));
					}
				}
			}
		}

		private void extractLoadTimeGenerators() {
			foreach (var e in _xmlsources) {
				foreach (var ge in e.Descendants("generator").Where(x => x.Attr("xmlload").ToBool())) {
					var code = ge.Id();
					var impl = Type.GetType(ge.Describe().Name).create<ILoadXmlGenerator>();
					_xmlloadgenerators[code] = impl;
				}
			}
		}

		private void resolveDepends() {
			log.debug("Thema Loader -> start resolve depends");
			var i = 0;
			foreach (var item in allitems.OfType<IFormThemaItem>()) {
				foreach (var l in item.InLockDepends) {
					l.Source = item;
					l.Target = (IFormThemaItem) requireThemaItem(l.TargetCode, l.ToString());
					l.Target.OutLockDepends.Add(l);
					i++;
				}
			}
			log.debug("Thema Loader -> " + i + " form depends  resolved");
		}

		private void resolveLibraries() {
			log.debug("Thema Loader -> start resolve libraries");
			var i = 0;
			foreach (var item in allitems) {
				foreach (var l in item.LibraryLinks) {
					l.Source = item;
					l.Target = requireThemaItem(l.TargetCode, l.ToString());
					l.Target.InLibraryUsage.Add(l);
					i++;
				}
			}
			log.debug("Thema Loader -> " + i + " library links resolved");
		}

		private void resolveLinks() {
			log.debug("Thema Loader -> start resolve links");
			var i = 0;
			foreach (var thema in allthemas) {
				foreach (var l in thema.OutLinks) {
					l.Source = thema;
					l.Target = requireThema(l.TargetCode, l.ToString());
					l.Target.InLinks.Add(l);
					i++;
				}
			}
			log.debug("Thema Loader -> " + i + " links resolved");
		}

		private IThema requireThema(string code, string message) {
			if (Factory.Themas.Index.ContainsKey(code)) return Factory.Themas.Index[code];
			throw new ThemaLoaderException("thema with code " + code + " required, but not exists : " + message);
		}

		private IThemaItem requireThemaItem(string code, string message) {
			var item = Factory.Themas.GetItem(code, null);
			if (null == item)
				throw new ThemaLoaderException("thema item with code " + code + " required, but not exists : " + message);
			return item;
		}

		private void prepareCoreIndex() {
			Factory.ExtraData = _extradata;
			log.debug("Thema Loader -> extra data setted");
			foreach (var themaelement in ThemaElements) {
				var thema = new Thema();
				thema.Factory = Factory;

				thema.XmlSource = themaelement;
				themaelement.Apply(thema);
				thema.SetupFromSourceXml();
				Factory.Themas.Index[thema.Code] = thema;
				log.debug("Thema Loader -> " + thema.Code + " thema added");
			}
		}

		private void extractElements() {
			log.debug("Thema Loader -> prepare thema and extra indexes");

			foreach (var e in XmlSources) {
				foreach (var x in e.Elements()) {
					if (x.Name.LocalName == "thema") {
						ThemaElements.Add(x);
					}
					else if (x.Name.LocalName == "extra") {
						_extradata.Add(x.Elements());
					}
					else {
						_extradata.Add(x);
					}
				}
			}
			log.debug("Thema Loader -> " + ThemaElements.Count + " thema elements detected");
			log.debug("Thema Loader -> " + _extradata.Elements().Count() + " extra elements detected");
		}

		private void loadSources() {
			log.debug("Thema Loader -> load xml sources");
			foreach (var source in Factory.Sources) {
				foreach (var e in source.GetXmlSources(Factory)) {
					XmlSources.Add(e);
				}
			}
			log.debug("Thema Loader -> " + XmlSources.Count + " sources loaded");
		}

		private void init() {
			_xmlsources = new List<XElement>();
			ThemaElements = new List<XElement>();
			_extradata = new XElement("extra");
		}
	}
}