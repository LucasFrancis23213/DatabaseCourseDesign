using Microsoft.AspNetCore.Mvc;
using SQLOperation.BusinessLogicLayer.ManagementFeatureBLL;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

namespace WebAppTest.APILayer.ManagementFeatureAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetUserInfoController : ControllerBase
    {
        private GetUserInfoBLL GetUserInfo;
        public GetUserInfoController()
        {
            GetUserInfo = new GetUserInfoBLL();
        }

        [HttpGet]
        public IActionResult CheckPassword(string UserName)
        {
            Tuple<bool, UserAccessibleInfo, string> result = GetUserInfo.GetInfo(UserName);
            if (result.Item1)
            {
                return Ok(result.Item2);
            }
            else
            {
                return BadRequest(result.Item3);
            }
        }
    }
}