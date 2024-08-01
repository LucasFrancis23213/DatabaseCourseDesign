using Microsoft.AspNetCore.Mvc;
using SQLOperation.BusinessLogicLayer.ManagementFeatureBLL;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

namespace WebAppTest.APILayer.ManagementFeatureAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetAPILogsController : ControllerBase
    {
        private GetAPILogsBLL GetAPILogsBLL;
        public GetAPILogsController()
        {
            GetAPILogsBLL = new();
        }

        [HttpGet]
        public IActionResult GetSystemLogs([FromQuery] QueryAPIAccessLogsArgs InputArgs)
        {
            Tuple<bool, string> result = GetAPILogsBLL.GetAPILogs(InputArgs);
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