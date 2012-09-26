using System;
using System.Runtime.Serialization;

namespace Comdiv.ThemaLoader {
	[Serializable]
	public class ThemaLoaderException : Exception {
		//
		// For guidelines regarding the creation of new exception types, see
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
		// and
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
		//

		public ThemaLoaderException() {
		}

		public ThemaLoaderException(string message) : base(message) {
		}

		public ThemaLoaderException(string message, Exception inner) : base(message, inner) {
		}

		protected ThemaLoaderException(
			SerializationInfo info,
			StreamingContext context) : base(info, context) {
		}
	}
}