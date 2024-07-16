using Microsoft.AspNetCore.Mvc;
using SQLOperation.BusinessLogicLayer.ManagementFeatureBLL;
using SQLOperation.PublicAccess.Utilities;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

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
        public IActionResult InsertUser([FromBody] RegisterUtil user)
        {
            if (user == null || string.IsNullOrEmpty(user.User_Name) || string.IsNullOrEmpty(user.Password) || string.IsNullOrEmpty(user.Contact))
            {
                return BadRequest("User data is incomplete");
            }

            // Convert UserDto object to Users object
            var userObj = new Users
            {
                User_Name = user.User_Name,
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
                if (result.Item2.Contains("ORA-00001"))
                    return StatusCode(500, result.Item2);

                return BadRequest(result.Item2);
            }
        }

    }
}