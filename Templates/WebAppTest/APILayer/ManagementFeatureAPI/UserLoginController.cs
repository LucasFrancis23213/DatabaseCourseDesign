using Microsoft.AspNetCore.Mvc;
using SQLOperation.BusinessLogicLayer.ManagementFeatureBLL;
using SQLOperation.PublicAccess.Utilities;

namespace WebAppTest.APILayer.ManagementFeatureAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserLoginController : ControllerBase
    {
        private UserLoginBLL UserLoginBLL;
        public UserLoginController() 
        {
            UserLoginBLL = new UserLoginBLL();
        }

        [HttpGet]
        public IActionResult GetUserInfo(string UserName)
        {
            Tuple<bool, Users, string> result = UserLoginBLL.GetUserInfoUtil(UserName);
            if (result.Item1)
            {
                return Ok(result.Item2);
            }
            else
            {
                return BadRequest(new { error = result.Item3 });
            }
        }
    }
}