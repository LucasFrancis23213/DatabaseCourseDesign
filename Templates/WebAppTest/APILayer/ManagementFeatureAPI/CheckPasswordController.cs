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
                return BadRequest(result.Item2);
            }
        }
    }
}