using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalFacade.ApiDto
{
    public class PropertyDto
    {
        public string Name { get; set; }

        public object DefaultValue { get; set; }

        public object UpdatedValue { get; set; }
    }
}
