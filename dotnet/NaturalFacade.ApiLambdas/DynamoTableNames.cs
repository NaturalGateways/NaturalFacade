using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.ApiLambdas
{
    public class DynamoTableNames : Services.IDynamoServiceTableNames
    {
        /// <summary>The singleton.</summary>
        private static DynamoTableNames s_singleton = null;
        /// <summary>The singleton.</summary>
        public static DynamoTableNames Singleton
        {
            get
            {
                if (s_singleton == null)
                    s_singleton = new DynamoTableNames();
                return s_singleton;
            }
        }

        /// <summary>Constructor.</summary>
        public DynamoTableNames()
        {
            this.ActionTableName = Environment.GetEnvironmentVariable("DbTableAction");
            this.ItemDataTableName = Environment.GetEnvironmentVariable("DbTableItemData");
            this.ItemLinkTableName = Environment.GetEnvironmentVariable("DbTableItemLink");
        }

        /// <summary>Services.IDynamoServiceTableNames implentation.</summary>
        public string ActionTableName { get; private set; }

        /// <summary>Services.IDynamoServiceTableNames implentation.</summary>
        public string ItemDataTableName { get; private set; }

        /// <summary>Services.IDynamoServiceTableNames implentation.</summary>
        public string ItemLinkTableName { get; private set; }
    }
}
