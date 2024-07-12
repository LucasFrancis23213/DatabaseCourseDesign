using Microsoft.AspNetCore.Mvc;
using SQLOperation.BusinessLogicLayer.ManagementFeatureBLL;
using SQLOperation.PublicAccess.Utilities;

namespace WebAppTest.APILayer.ManagementFeatureAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private RegisterBLL RegisterBLL;
        public RegisterController()
        {
            RegisterBLL = new RegisterBLL();
        }

        [HttpPost]
        public IActionResult InsertUser([FromBody] dynamic user)
        {
            if (user == null || user.UserName == null || user.Password == null || user.Contact == null)
            {
                return BadRequest("User data is incomplete");
            }

            // Convert dynamic object to Users object
            var userObj = new Users
            {
                User_Name = user.UserName,
                Password = user.Password,
                Contact = user.Contact,
            };

            var result = RegisterBLL.InsertUser(userObj);

            if (result.Item1)
            {
                return Ok("User successfully inserted");
            }
            else
            {
                return BadRequest(result.Item2);
            }
        }
    }
}