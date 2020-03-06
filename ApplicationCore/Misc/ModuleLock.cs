using System;
using NeoSmart.AsyncLock;

namespace ApplicationCore.Misc
{
    public class ModuleLock
    {
        public static AsyncLock GetAsyncLock()
        {
            return new AsyncLock();
        }
    }

}
