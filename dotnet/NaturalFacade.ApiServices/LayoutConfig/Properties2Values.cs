using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.LayoutConfig
{
    public static class Properties2Values
    {
        public static object[] GetValuesFromProperties(ApiDto.PropertyDto[] propertyArray)
        {
            if (propertyArray?.Any() ?? false)
            {
                return propertyArray.Select(x => x.UpdatedValue ?? x.DefaultValue).ToArray();
            }
            else
            {
                return null;
            }
        }
    }
}
