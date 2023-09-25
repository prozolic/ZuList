using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZuList.Internal
{
    internal class FastListDebugView<T>
    {
        private readonly FastList<T> _fastList;

        public FastListDebugView(FastList<T> fastList)
        {
            ErrorHelper.ThrowArgumentNullException(fastList, nameof(fastList));

            _fastList = fastList;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
            => _fastList.ToArray();
    }
}
