using System.Collections.Generic;

namespace Comdiv.ThemaLoader {
	public interface IFormThemaItem : IThemaItem {
		string LockCode { get; set; }
		List<IFormLockDepend> InLockDepends { get; }
		List<IFormLockDepend> OutLockDepends { get; }
	}
}