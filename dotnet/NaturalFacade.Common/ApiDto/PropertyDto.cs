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
        public string ValueType { get; set; }

        public string Name { get; set; }

        public object DefaultValue { get; set; }

        public object UpdatedValue { get; set; }

        public int? TimerDirection { get; set; }

        public long? TimerMinValue { get; set; }

        public long? TimerMaxValue { get; set; }
    }
}
