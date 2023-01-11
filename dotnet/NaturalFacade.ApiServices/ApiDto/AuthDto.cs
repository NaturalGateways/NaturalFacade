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
        CreateLayout,
        GetLayout,
        GetLayoutControls,
        PutLayout,
        PutLayoutPropertyValues
    }

    public class AuthRequestPayloadDto
    {
        public string RequestType { get; set; }

        public AuthUpdateCurrentUserRequestDto UpdateCurrentUser { get; set; }

        public AuthCreateLayoutRequestDto CreateLayout { get; set; }

        public AuthGetLayoutRequestDto GetLayout { get; set; }

        public AuthGetLayoutControlsRequestDto GetLayoutControls { get; set; }

        public AuthPutLayoutRequestDto PutLayout { get; set; }

        public AuthPutLayoutPropertyValuesRequestDto PutLayoutPropertyValues { get; set; }
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

    #region GetLayout

    public class AuthGetLayoutRequestDto
    {
        public string LayoutId { get; set; }
    }

    #endregion

    #region GetLayoutControls

    public class AuthGetLayoutControlsRequestDto
    {
        public string LayoutId { get; set; }

        public int ControlsIndex { get; set; }
    }

    #endregion

    #region PutLayout

    public class AuthPutLayoutRequestDto
    {
        public string LayoutId { get; set; }

        public LayoutConfig.LayoutConfig LayoutConfig { get; set; }
    }

    #endregion

    #region PutLayoutProperties

    public class AuthPutLayoutPropertyValuesRequestDto
    {
        public string LayoutId { get; set; }

        public AuthPutLayoutPropertyValueDataRequestDto[] Values { get; set; }
    }

    public class AuthPutLayoutPropertyValueDataRequestDto
    {
        public int PropertyIndex { get; set; }

        public object Value { get; set; }
    }

    #endregion
}
