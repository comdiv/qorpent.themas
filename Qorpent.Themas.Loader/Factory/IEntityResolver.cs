using System;

namespace Comdiv.ThemaLoader {
	public interface IEntityResolver {
		T Get<T>(object key);
		object Get(Type type, object key);
	}
}