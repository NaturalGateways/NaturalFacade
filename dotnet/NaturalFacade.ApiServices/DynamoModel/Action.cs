using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.ActionModel
{
    public enum ActionType
    {
        PutLayout
    }

    public class Action
    {
        public ActionType AuthType { get; set; }

        public ActionLayout Layout { get; set; }
    }

    public class ActionLayout
    {
        public string UserId { get; set; }

        public string LayoutId { get; set; }

        public LayoutConfig.LayoutConfig Config { get; set; }
    }
}
