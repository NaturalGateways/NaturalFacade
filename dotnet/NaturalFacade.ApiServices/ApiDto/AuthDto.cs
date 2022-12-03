using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.ApiDto
{
    #region Base

    public class AuthRequestDto : CommonDto<AuthRequestPayloadDto>
    {
        //
    }

    public enum AuthRequestType
    {
        GetCurrentUser,
        UpdateCurrentUser,
        GetLayoutSummaryPage,
        CreateLayout
    }

    public class AuthRequestPayloadDto
    {
        public string RequestType { get; set; }

        public AuthUpdateCurrentUserRequestDto UpdateCurrentUser { get; set; }

        public AuthCreateLayoutRequestDto CreateLayout { get; set; }
    }

    #endregion

    #region UpdateCurrentUser

    public class AuthUpdateCurrentUserRequestDto
    {
        public string Name { get; set; }
    }

    #endregion

    #region CreateLayout

    public class AuthCreateLayoutRequestDto
    {
        public string Name { get; set; }
    }

    #endregion
}
