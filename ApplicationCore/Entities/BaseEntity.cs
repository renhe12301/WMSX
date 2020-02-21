using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using ApplicationCore.Misc;

namespace ApplicationCore.Entities
{
    public class BaseEntity
    {
        public int Id { get; set; }

        public string MD5String()
        {
            StringBuilder md5 = new StringBuilder();
            Type type = this.GetType();
            List<Attribute> attributes = new List<Attribute>();
            var members = type.GetProperties();
            Dictionary<string,string> fileds = new Dictionary<string, string>();
            foreach (var member in members)
            {
                var attr = member.GetCustomAttribute(typeof(DescriptionAttribute));
                if (attr != null)
                {
                    DescriptionAttribute desc = attr as DescriptionAttribute;
                    fileds[desc.Description] = member.GetValue(this).ToString();
                }
            }

            var orderFields = fileds.OrderBy(f => f.Key);
            foreach (var o in orderFields)
                md5.Append(o.Value);
            var result = MD5Gen.Generate(md5.ToString());
            return result;
        }
        
    
    }
}
