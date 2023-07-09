using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.Xml
{
    /// <summary>Interface for the object passed in to read attributes.</summary>
    public interface ITagAttributes
    {
        /// <summary>Getter for a string, throws an exception if it is missing.</summary>
        string GetString(string name);

        /// <summary>Getter for a string, returns null if it is missing.</summary>
        string? GetNullableString(string name);
        /// <summary>Getter for an enum, returns null if it is missing.</summary>
        EnumType? GetNullableEnum<EnumType>(string name) where EnumType : struct, IConvertible;
    }
}
