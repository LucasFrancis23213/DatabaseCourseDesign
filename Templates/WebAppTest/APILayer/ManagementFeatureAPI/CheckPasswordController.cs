using Microsoft.AspNetCore.Mvc;
using SQLOperation.BusinessLogicLayer.ManagementFeatureBLL;

namespace WebAppTest.APILayer.ManagementFeatureAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckPasswordController : ControllerBase
    {
        private UserLoginBLL UserLoginBLL;
        public CheckPasswordController() 
        {
            UserLoginBLL = new UserLoginBLL();
        }

        [HttpGet]
        public IActionResult CheckPassword(string UserName, string Password)
        {
            Tuple<bool, string> result = UserLoginBLL.CheckPassword(UserName, Password);
            if (result.Item1)
            {
                return Ok(result.Item2);
            }
            else
            {
                switch (result.Item2)
                {
                    case "用户名与密码不匹配":
                        return Unauthorized(result.Item2);
                    case "Users表没有符合要求的元素":
                        return NotFound(result.Item2);
                    default:
                        return BadRequest(result.Item2);
                }
            }
        }
    }
}