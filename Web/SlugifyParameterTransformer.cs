using Microsoft.AspNetCore.Routing;
using System.Text.RegularExpressions;

namespace Web
{
    /// <summary>
    /// 转换url地址
    /// </summary>
    public class SlugifyParameterTransformer : IOutboundParameterTransformer
    {
        public string TransformOutbound(object value)
        {
            if (value == null) { return null; }

            // Slugify value
            return Regex.Replace(value.ToString(), "([a-z])([A-Z])", "$1-$2").ToLower();
        }
    }
}
