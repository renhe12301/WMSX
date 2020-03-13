
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;


namespace Web
{
    public class GeneralEngine : IEngine
    {
        private IServiceProvider _provider;
        public GeneralEngine(IServiceProvider provider)
        {
            this._provider = provider;
        }
        /// <summary>
        /// 构建实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Resolve<T>()
        {
            return _provider.GetService<T>();
        }
    }
}
