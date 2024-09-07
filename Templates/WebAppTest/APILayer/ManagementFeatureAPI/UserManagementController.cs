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
        private readonly UserAuthBLL _userAuthBLL;
        private readonly DeleteAuthInfoBLL _deleteAuthInfoBLL;
        private readonly GetAuthInfoBLL _getAuthInfoBLL;

        public UserManagementController()
        {
            _getUserInfoBLL = new GetUserInfoBLL();
            _updateUserInfoBLL = new UpdateUserInfoBLL();
            _registerBLL = new RegisterBLL();
            _deleteUserBLL = new DeleteUserBLL();
            _userLoginBLL = new UserLoginBLL();
            _userAuthBLL = new UserAuthBLL();
            _deleteAuthInfoBLL = new DeleteAuthInfoBLL();
            _getAuthInfoBLL = new GetAuthInfoBLL();
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
        public IActionResult UpdateUserInfo([FromBody] UpdateUserInfoUtil NewInfo)
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
                    "未找到用户" => NotFound(result.Item2),
                    _ => BadRequest(result.Item2),
                    
                };
            }
        }

        [HttpPost("NewUserAuthed")]
        public IActionResult NewUserAuthed([FromBody] Auth_Info NewAuth)
        {
            var result = _userAuthBLL.NewUserAuthed(NewAuth);
            if (result.Item1)
            {
                return Ok(result.Item2);
            }
            else
            {
                return BadRequest(result.Item2);
            }
        }

        // 此接口调用后不保证有实际删除操作，但是保证操作之后数据库中不再存在传入用户的认证
        [HttpDelete("DeleteAuthInfo")]
        public IActionResult DeleteAuthInfo(int UserID)
        {
            var result = _deleteAuthInfoBLL.DeleteAuthInfo(UserID);
            if (result.Item1)
            {
                return Ok(result.Item2);
            }
            else
            {
                return BadRequest(result.Item2);
            }
        }

        [HttpGet("GetAuthInfo")]
        public IActionResult GetAuthInfo(int? UserID)
        {
            var result = _getAuthInfoBLL.GetAuthInfo(UserID);
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
