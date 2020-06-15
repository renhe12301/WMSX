using System;
using NeoSmart.AsyncLock;

namespace ApplicationCore.Misc
{
    public class ModuleLock
    {
        private static AsyncLock _asyncLock = new AsyncLock();
        private static AsyncLock _asyncLock2 = new AsyncLock();
        private static AsyncLock _asyncLock3 = new AsyncLock();
        private static AsyncLock _asyncLock4 = new AsyncLock();
        private static AsyncLock _asyncLock5 = new AsyncLock();
        private static AsyncLock _asyncLock6 = new AsyncLock();
        private static AsyncLock _asyncLock7 = new AsyncLock();
        private static AsyncLock _asyncLock8 = new AsyncLock();
        private static AsyncLock _asyncLock9 = new AsyncLock();
        private static AsyncLock _asyncLock10 = new AsyncLock();
        public static AsyncLock GetAsyncLock()
        {
            return _asyncLock;
        }
        
        public static AsyncLock GetAsyncLock2()
        {
            return _asyncLock2;
        }
        public static AsyncLock GetAsyncLock3()
        {
            return _asyncLock3;
        }
        public static AsyncLock GetAsyncLock4()
        {
            return _asyncLock4;
        }
        public static AsyncLock GetAsyncLock5()
        {
            return _asyncLock5;
        }
        public static AsyncLock GetAsyncLock6()
        {
            return _asyncLock6;
        }
        public static AsyncLock GetAsyncLock7()
        {
            return _asyncLock7;
        }
        public static AsyncLock GetAsyncLock8()
        {
            return _asyncLock8;
        }
        public static AsyncLock GetAsyncLock9()
        {
            return _asyncLock9;
        }
        public static AsyncLock GetAsyncLock10()
        {
            return _asyncLock10;
        }
    }

}
