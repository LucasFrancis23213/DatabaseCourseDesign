using Microsoft.AspNetCore.Mvc;
using SQLOperation.BusinessLogicLayer.ManagementFeatureBLL;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

namespace WebAppTest.APILayer.ManagementFeatureAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetSecurityEventsController : ControllerBase
    {
        private GetSecurityEventsBLL GetSecurityEventsBLL;
        public GetSecurityEventsController()
        {
            GetSecurityEventsBLL = new();
        }

        [HttpGet]
        public IActionResult GetUserOpsLogs([FromQuery] QuerySecurityEventsArgs InputArgs)
        {
            Tuple<bool, string> result = GetSecurityEventsBLL.GetSecurityEvents(InputArgs);
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