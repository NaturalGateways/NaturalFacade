using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.App
{
    public interface IApiService
    {
        /// <summary>Getter for the user details after authenticating.</summary>
        ApiModel.UserDetailsResponse GetUserDetails(AuthStateApiAccess apiAccess);
    }
}
