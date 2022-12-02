using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.SandboxConsole.Helpers
{
    public class DynamoTableNames : Services.IDynamoServiceTableNames
    {
        /// <summary>Services.IDynamoServiceTableNames implentation.</summary>
        public string ActionTableName { get { return "Actions"; } }

        /// <summary>Services.IDynamoServiceTableNames implentation.</summary>
        public string ItemDataTableName { get { return "ItemData"; } }

        /// <summary>Services.IDynamoServiceTableNames implentation.</summary>
        public string ItemLinkTableName { get { return "ItemLinks"; } }
    }
}
