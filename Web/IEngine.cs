using System;
using System.Collections.Generic;
using System.Text;

namespace Web
{
    public interface IEngine
    {
        T Resolve<T>();
    }
}