using System;
using NeoSmart.AsyncLock;

namespace ApplicationCore.Misc
{
    public class ModuleLock
    {
        private static AsyncLock _asyncLock = new AsyncLock();
        public static AsyncLock GetAsyncLock()
        {
            return _asyncLock;
        }
    }

}
