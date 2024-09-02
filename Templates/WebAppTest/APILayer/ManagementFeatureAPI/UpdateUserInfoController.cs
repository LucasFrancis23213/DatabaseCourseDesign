using SQLOperation.BusinessLogicLayer.ManagementFeatureBLL;
using Microsoft.AspNetCore.Mvc;
using SQLOperation.PublicAccess.Utilities;

namespace WebAppTest.APILayer.ManagementFeatureAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateUserInfoController : ControllerBase
    {
        private UpdateUserInfoBLL UpdateUserInfoBLL;

        public UpdateUserInfoController()
        {
            UpdateUserInfoBLL = new UpdateUserInfoBLL();
        }

        [HttpPost]
        public IActionResult UpdateUserInfo(Users NewInfo)
        {
            var (QueryResult, Message) = UpdateUserInfoBLL.UpdateUserInfo(NewInfo);
            if (QueryResult)
            {
                return Ok(Message);
            }

            switch (Message)
            {
                case "δ�ҵ��û�":
                    return NotFound(Message);
                default:
                    return BadRequest(Message);
            }
        }
    }
}