using Microsoft.AspNetCore.Mvc;
using SQLOperation.BusinessLogicLayer.ManagementFeatureBLL;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

namespace WebAppTest.APILayer.ManagementFeatureAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetUserInfoController : ControllerBase
    {
        private GetUserInfoBLL GetUserInfoBLL;
        public GetUserInfoController()
        {
            GetUserInfoBLL = new GetUserInfoBLL();
        }

        [HttpGet]
        public IActionResult GetInfoByUserName(string UserName)
        {
            Tuple<bool, UserAccessibleInfo, string> result = GetUserInfoBLL.GetInfo(UserName);
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