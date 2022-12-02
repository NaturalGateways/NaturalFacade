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
        UpdateCurrentUser
    }

    public class AuthRequestPayloadDto
    {
        public string RequestType { get; set; }

        public AuthUpdateCurrentUserRequestDto UpdateCurrentUser { get; set; }
    }

    #endregion

    #region UpdateCurrentUser

    public class AuthUpdateCurrentUserRequestDto
    {
        public string Name { get; set; }
    }

    #endregion
}
