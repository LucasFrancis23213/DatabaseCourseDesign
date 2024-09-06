using Microsoft.AspNetCore.Mvc;
using SQLOperation.BusinessLogicLayer.ManagementFeatureBLL;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

namespace WebAppTest.APILayer.ManagementFeatureAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsQueryController : ControllerBase
    {
        private readonly GetUserOpsLogsInfoBLL _getUserOpsLogsInfoBLL;
        private readonly GetSecurityEventsBLL _getSecurityEventsBLL;
        private readonly GetSystemLogsBLL _getSystemLogsBLL;
        private readonly GetAPILogsBLL _getAPILogsBLL;
        private readonly GetNotificationLogsBLL _getNotificationLogsBLL;
        private readonly GetRecommendationLogsBLL _getRecommendationLogsBLL;

        public LogsQueryController()
        {
            _getAPILogsBLL = new GetAPILogsBLL();
            _getUserOpsLogsInfoBLL = new GetUserOpsLogsInfoBLL();
            _getSecurityEventsBLL = new GetSecurityEventsBLL();
            _getSystemLogsBLL = new GetSystemLogsBLL();
            _getNotificationLogsBLL = new GetNotificationLogsBLL();
            _getRecommendationLogsBLL = new GetRecommendationLogsBLL();
        }

        [HttpGet("APILogs")]
        public IActionResult GetAPILogs([FromQuery] QueryAPIAccessLogsArgs inputArgs)
        {
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
