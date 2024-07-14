using Microsoft.AspNetCore.Mvc;
using SQLOperation.BusinessLogicLayer.ManagementFeatureBLL;
using SQLOperation.PublicAccess.Utilities;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

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
        public IActionResult GetUserInfo(string UserName, string Password)
        {
            Tuple<bool, Users, string> result = UserLoginBLL.GetUserInfoUtil(UserName);
            if (result.Item1)
            {
                if (result.Item2.Password == Password) 
                {
                    var retval = new UserInfoAfterLogin();
                    retval.UserID = result.Item2.User_ID;
                    retval.UserName = result.Item2.User_Name;
                    retval.Contact = result.Item2.Contact;
                    return Ok(retval); 
                }
                else
                {
                    return BadRequest(new { error = "用户名与密码不匹配"} );
                }
            }
            else
            {
                return BadRequest(new { error = result.Item3 });
            }
        }
    }
}