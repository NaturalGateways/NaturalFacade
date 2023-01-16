using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalFacade.ApiDto
{
    public enum PropertyTypeDto
    {
        String,
        Boolean,
        Timer
    }

    public class PropertyDto
    {
        public PropertyTypeDto Type { get; set; }

        public string Name { get; set; }

        public object Value { get; set; }
    }
}
