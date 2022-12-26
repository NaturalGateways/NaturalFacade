using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalFacade.ApiDto
{
    public enum PropertyTypeDto
    {
        String,
        Boolean
    }

    public class PropertyDto
    {
        public PropertyTypeDto ValueType { get; set; }

        public string Name { get; set; }

        public object DefaultValue { get; set; }

        public object UpdatedValue { get; set; }
    }
}
