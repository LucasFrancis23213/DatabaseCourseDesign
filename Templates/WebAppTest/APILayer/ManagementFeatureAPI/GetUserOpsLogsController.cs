using Microsoft.AspNetCore.Mvc;
using SQLOperation.BusinessLogicLayer.ManagementFeatureBLL;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

namespace WebAppTest.APILayer.ManagementFeatureAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetUserOpsLogsController : ControllerBase
    {
        private GetUserOpsLogsInfoBLL GetUserOpsLogsInfoBLL;
        public GetUserOpsLogsController()
        {
            GetUserOpsLogsInfoBLL = new();
        }

        [HttpGet]
        public IActionResult GetUserOpsLogs([FromQuery] QueryUserOpsLogsArgs InputArgs)
        {
            Tuple<bool, string> result = GetUserOpsLogsInfoBLL.GetUserOpsLogs(InputArgs);
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