using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.ActionModel
{
    public enum ActionType
    {
        CreateUser,
        UpdateUser
    }

    public class Action
    {
        public ActionType AuthType { get; set; }

        public ActionCreateUser CreateUser { get; set; }

        public ActionUpdateUser UpdateUser { get; set; }

        public object AsMinimalObject()
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            switch (this.AuthType)
            {
                case ActionType.CreateUser:
                    props.Add("CreateUser", this.CreateUser);
                    break;
                case ActionType.UpdateUser:
                    props.Add("UpdateUser", this.UpdateUser);
                    break;
                default:
                    throw new Exception("Cannot create minimal object of action.");
            }
            return props;
        }
    }

    public class ActionCreateUser
    {
        public string UserId { get; set; }

        public string Email { get; set; }
    }

    public class ActionUpdateUser
    {
        public string UserId { get; set; }

        public string Name { get; set; }
    }
}
