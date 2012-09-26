using System.Collections.Generic;
using System.Linq;
using Comdiv.ThemaLoader.Wrap;

namespace Comdiv.ThemaLoader.UI {
	public class UserThemaTreeBuilder : IUserThemaTreeBuilder {
		#region IUserThemaTreeBuilder Members

		public ThemaFactory Factory { get; set; }

		public UserThemaTree BuildTree(string usr = null, WrapContext context = null) {
			var result = new UserThemaTree();
			result.Usr = usr;
			result.Context = context;
			var groups = new List<utThemaGroup>();
			foreach (var g in Factory.Themas.GetGroups(usr)) {
				var ug = new utThemaGroup();
				ug.Idx = g.EffectiveIndex;
				ug.Code = g.Code;
				ug.Name = g.Name;
				ug.Target = g;
				ug.Tree = result;

				var roots = new List<utThemaRoot>();
				foreach (var rt in g.GroupMembers.Where(x => x.Authorized(usr) && x.Parent == null).OrderBy(x => x.EffectiveIndex)) {
					var root = new utThemaRoot();
					root.Idx = rt.EffectiveIndex;
					root.Code = rt.Code;
					root.Name = rt.Name;
					root.Target = rt;
					root.Usr = usr;
					root.Group = ug;
					var terminals = new List<utTerminal>();

					foreach (var c in rt.Children.Where(x => x.Authorized(usr)).OrderBy(x => x.EffectiveIndex)) {
						var terminal = new utTerminal();
						terminal.Idx = c.EffectiveIndex;
						terminal.Code = c.Code;
						terminal.Name = c.Name;
						terminal.Target = c;
						terminals.Add(terminal);
						terminal.Usr = usr;
						terminal.Parent = root;
					}

					root.Terminals = terminals.ToArray();
					roots.Add(root);
				}

				ug.Roots = roots.ToArray();

				groups.Add(ug);
			}
			result.Groups = groups.ToArray();
			return result;
		}

		#endregion
	}
}