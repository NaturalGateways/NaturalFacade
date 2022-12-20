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
        UpdateUser,
        CreateLayout,
        PutLayout
    }

    public class Action
    {
        public ActionType AuthType { get; set; }

        public ActionCreateUser CreateUser { get; set; }

        public ActionUpdateUser UpdateUser { get; set; }

        public ActionCreateLayout CreateLayout { get; set; }

        public ActionPutLayout PutLayout { get; set; }

        public object AsMinimalObject()
        {
            switch (this.AuthType)
            {
                case ActionType.CreateUser:
                    return this.CreateUser;
                case ActionType.UpdateUser:
                    return this.UpdateUser;
                case ActionType.CreateLayout:
                    return this.CreateLayout;
                case ActionType.PutLayout:
                    return this.PutLayout;
                default:
                    throw new Exception("Cannot create minimal object of action.");
            }
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

    public class ActionCreateLayout
    {
        public string UserId { get; set; }

        public string LayoutId { get; set; }

        public string Name { get; set; }
    }

    public class ActionPutLayout
    {
        public string LayoutId { get; set; }

        public LayoutConfig.LayoutConfig LayoutConfig { get; set; }
    }
}
