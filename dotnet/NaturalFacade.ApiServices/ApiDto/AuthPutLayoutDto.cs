using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.ApiDto
{
    public class AuthPutLayoutRequestDto
    {
        public string LayoutId { get; set; }

        public LayoutConfig.LayoutConfig Config { get; set; }
    }

    public class AuthPutLayoutResponseDto
    {
        public string LayoutId { get; set; }
    }
}
