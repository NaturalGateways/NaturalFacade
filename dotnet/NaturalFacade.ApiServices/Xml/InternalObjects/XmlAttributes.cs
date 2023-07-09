using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.Xml.InternalObjects
{
    internal class XmlAttributes : ITagAttributes
    {
        #region Base

        /// <summary>The XML reader.</summary>
        private System.Xml.XmlReader m_reader = null;

        /// <summary>Constructor.</summary>
        public XmlAttributes(System.Xml.XmlReader reader)
        {
            m_reader = reader;
        }

        #endregion

        #region ITagAttributes implementation

        /// <summary>Getter for a string, throws an exception if it is missing.</summary>
        public string GetString(string name)
        {
            string? attributeValue = m_reader.GetAttribute(name);
            if (attributeValue == null)
                throw new Exception($"Tag '{m_reader.Name}' has no attribute '{name}'.");
            return attributeValue;
        }

        /// <summary>Getter for a string, returns null if it is missing.</summary>
        public string? GetNullableString(string name)
        {
            string? attributeValue = m_reader.GetAttribute(name);
            if (attributeValue == null)
                return null;
            return attributeValue;
        }

        /// <summary>Getter for an enum, returns null if it is missing.</summary>
        public EnumType? GetNullableEnum<EnumType>(string name) where EnumType : struct, IConvertible
        {
            string? attributeValue = m_reader.GetAttribute(name);
            if (attributeValue == null)
                return null;
            EnumType enumValue = default(EnumType);
            if (Enum.TryParse<EnumType>(attributeValue, out enumValue))
            {
                return enumValue;
            }
            return null;
        }

        #endregion
    }
}
