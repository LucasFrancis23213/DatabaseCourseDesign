using Microsoft.AspNetCore.Mvc;
using SQLOperation.BusinessLogicLayer.ManagementFeatureBLL;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

namespace WebAppTest.APILayer.ManagementFeatureAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetSystemLogsController : ControllerBase
    {
        private GetSystemLogsBLL GetSystemLogsBLL;
        public GetSystemLogsController()
        {
            GetSystemLogsBLL = new();
        }

        [HttpGet]
        public IActionResult GetSystemLogs([FromQuery] QuerySystemLogsArgs InputArgs)
        {
            Tuple<bool, string> result = GetSystemLogsBLL.GetSystemLogs(InputArgs);
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