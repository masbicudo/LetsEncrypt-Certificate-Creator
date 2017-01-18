using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace LetsEncryptAcmeReg
{
    public static class DynamicExtensions
    {
        public static IDictionary ToDictionary(this object value)
        {
            if (value == null)
                return null;

            var dic = new Dictionary<string, object>();

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(value.GetType()))
                dic.Add(property.Name, property.GetValue(value));

            return dic;
        }
    }
}