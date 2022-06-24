using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.ApiDto
{
    public class AuthGetCurrentUserResponseDto
    {
        public string userId { get; set; }

        public string email { get; set; }

        public string name { get; set; }
    }
}
