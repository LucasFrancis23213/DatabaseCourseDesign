using Microsoft.AspNetCore.Mvc;
using SQLOperation.BusinessLogicLayer.ManagementFeatureBLL;

namespace WebAppTest.APILayer.ManagementFeatureAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeleteUserController : ControllerBase
    {
        private DeleteUserBLL DeleteUserBLL;
        public DeleteUserController()
        {
            DeleteUserBLL = new DeleteUserBLL();
        }

        [HttpGet]
        public IActionResult CheckPassword(string UserName)
        {
            Tuple<bool, string> result = DeleteUserBLL.DeleteUser(UserName);
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