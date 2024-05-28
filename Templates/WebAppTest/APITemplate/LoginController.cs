using Microsoft.AspNetCore.Mvc;
using SQLOperation.PublicAccess.Templates.UserManager;
using SQLOperation.PublicAccess.Utilities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAppTest.APITemplate
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private CreateUser CreateUser;

        [HttpPost("CreateAccount")]

        public IActionResult CreateAccount([FromBody] UserInfo UserInfo)
        {
            try
            {
                CreateUser = new CreateUser(UserInfo);
                bool CreateStatus = CreateUser.CreateStatus;
                if (CreateStatus)
                    return Ok("用户创建成功");
                else
                    return BadRequest($"用户创建失败，原因：{CreateUser.ReasonForCreationFailure}");
            }
            catch (Exception ex)
            {
                return BadRequest($"报错：{ex}");
            }
        }


    }
}
