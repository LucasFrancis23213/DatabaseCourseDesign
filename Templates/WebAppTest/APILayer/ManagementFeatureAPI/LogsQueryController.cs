using Microsoft.AspNetCore.Mvc;
using SQLOperation.BusinessLogicLayer.ManagementFeatureBLL;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

namespace WebAppTest.APILayer.ManagementFeatureAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsQueryController : ControllerBase
    {
        [HttpGet("APILogs")]
        public IActionResult GetAPILogs([FromQuery] QueryAPIAccessLogsArgs inputArgs)
        {
            GetAPILogsBLL _getAPILogsBLL = new();
            var result = _getAPILogsBLL.GetAPILogs(inputArgs);
            if (result.Item1)
            {
                return Ok(result.Item2);
            }
            else
            {
                return BadRequest(result.Item2);
            }
        }

        [HttpGet("UserOpsLogs")]
        public IActionResult GetUserOpsLogs([FromQuery] QueryUserOpsLogsArgs inputArgs)
        {
            GetUserOpsLogsInfoBLL _getUserOpsLogsInfoBLL = new();
            var result = _getUserOpsLogsInfoBLL.GetUserOpsLogs(inputArgs);
            if (result.Item1)
            {
                return Ok(result.Item2);
            }
            else
            {
                return BadRequest(result.Item2);
            }
        }

        [HttpGet("SecurityEvents")]
        public IActionResult GetSecurityEvents([FromQuery] QuerySecurityEventsArgs inputArgs)
        {
            GetSecurityEventsBLL _getSecurityEventsBLL = new();
            var result = _getSecurityEventsBLL.GetSecurityEvents(inputArgs);
            if (result.Item1)
            {
                return Ok(result.Item2);
            }
            else
            {
                return BadRequest(result.Item2);
            }
        }

        [HttpGet("SystemLogs")]
        public IActionResult GetSystemLogs([FromQuery] QuerySystemLogsArgs inputArgs)
        {
            GetSystemLogsBLL _getSystemLogsBLL = new();
            var result = _getSystemLogsBLL.GetSystemLogs(inputArgs);
            if (result.Item1)
            {
                return Ok(result.Item2);
            }
            else
            {
                return BadRequest(result.Item2);
            }
        }

        [HttpGet("NotificationLogs")]
        public IActionResult GetNotificationLogs([FromQuery] QueryNotificationLogsArgs inputArgs)
        {
            GetNotificationLogsBLL _getNotificationLogsBLL = new();
            var result = _getNotificationLogsBLL.GetLogs(inputArgs);
            if (result.Item1)
            {
                return Ok(result.Item2);
            }
            else
            {
                return BadRequest(result.Item2);
            }
        }

        [HttpGet("RecommendationLogs")]
        public IActionResult GetRecommendationLogs([FromQuery] QueryRecommendationLogsArgs inputArgs)
        {
            GetRecommendationLogsBLL _getRecommendationLogsBLL = new();
            var result = _getRecommendationLogsBLL.GetLogs(inputArgs);
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
