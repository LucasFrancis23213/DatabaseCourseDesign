using Microsoft.AspNetCore.Mvc;
using SQLOperation.BusinessLogicLayer.ManagementFeatureBLL;
using SQLOperation.PublicAccess.Utilities;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

namespace WebAppTest.APILayer.ManagementFeatureAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagementController : ControllerBase
    {
        private readonly GetUserInfoBLL _getUserInfoBLL;
        private readonly UpdateUserInfoBLL _updateUserInfoBLL;
        private readonly RegisterBLL _registerBLL;
        private readonly DeleteUserBLL _deleteUserBLL;
        private readonly UserLoginBLL _userLoginBLL;

        public UserManagementController()
        {
            _getUserInfoBLL = new GetUserInfoBLL();
            _updateUserInfoBLL = new UpdateUserInfoBLL();
            _registerBLL = new RegisterBLL();
            _deleteUserBLL = new DeleteUserBLL();
            _userLoginBLL = new UserLoginBLL();
        }

        [HttpGet("UserGetUserInfo")]
        public IActionResult UserGetUserInfo(int? UserID, string? UserName)
        {
            var result = _getUserInfoBLL.GetInfo(UserID, UserName);
            if (result.Item1)
            {
                return Ok(result.Item2);
            }
            else
            {
                return BadRequest(result.Item2);
            }
        }

        [HttpGet("AdminGetUserInfo")]
        public IActionResult AdminGetUserInfo(int? UserID, string? UserName)
        {
            var result = _getUserInfoBLL.GetInfo(UserID, UserName, true);
            if (result.Item1)
            {
                return Ok(result.Item2);
            }
            else
            {
                return BadRequest(result.Item2);
            }
        }

        [HttpPut("UpdateUserInfo")]
        public IActionResult UpdateUserInfo([FromBody] Users NewInfo)
        {
            var (QueryResult, Message) = _updateUserInfoBLL.UpdateUserInfo(NewInfo);
            if (QueryResult)
            {
                return Ok(Message);
            }

            return Message switch
            {
                "未找到用户" => NotFound(Message),
                _ => BadRequest(Message),
            };
        }

        [HttpPost("RegisterUser")]
        public IActionResult RegisterUser([FromBody] RegisterUtil user)
        {
            if (user == null || string.IsNullOrEmpty(user.User_Name) || string.IsNullOrEmpty(user.Password) || string.IsNullOrEmpty(user.Contact))
            {
                return BadRequest("User data is incomplete");
            }

            var userObj = new Users
            {
                User_Name = user.User_Name,
                Password = user.Password,
                Contact = user.Contact,
            };

            var result = _registerBLL.InsertUser(userObj);

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

        [HttpDelete("DeleteUser")]
        public IActionResult DeleteUser(int UserID)
        {
            var result = _deleteUserBLL.DeleteUser(UserID);
            if (result.Item1)
            {
                return Ok(result.Item2);
            }
            else
            {
                return BadRequest(result.Item2);
            }
        }

        [HttpGet("CheckPassword")]
        public IActionResult CheckPassword(string UserName, string Password)
        {
            var result = _userLoginBLL.CheckPassword(UserName, Password);
            if (result.Item1)
            {
                return Ok(result.Item2);
            }
            else
            {
                return result.Item2 switch
                {
                    "用户名与密码不匹配" => Unauthorized(result.Item2),
                    "Users表没有符合要求的元素" => NotFound(result.Item2),
                    _ => BadRequest(result.Item2),
                };
            }
        }
    }
}
