using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime;
using System.Text;

namespace Web
{
    public class EnginContext
    {
        private static IEngine _engine;
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IEngine initialize(IEngine engine)
        {
            if (_engine == null)
            {
                _engine = engine;
            }
            return _engine;
        }

        public static IEngine Current
        {
            get
            {
                return _engine;
            }
        }
    }
}
