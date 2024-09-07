using Microsoft.AspNetCore.Mvc;
using SQLOperation.BusinessLogicLayer.ManagementFeatureBLL;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

namespace WebAppTest.APILayer.ManagementFeatureAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsInsertController : ControllerBase
    {
        [HttpPost("APILogs")]
        public IActionResult InsertAPILog([FromBody] APIAccessLogsInsertUtil newLog)
        {
            var _insertAPILogsBLL = new InsertAPILogsBLL();
            var result = _insertAPILogsBLL.InsertLog(newLog);
            if (result.Item1)
            {
                return Ok("Successfully inserted");
            }
            else
            {
                return BadRequest(result.Item2);
            }
        }

        [HttpPost("SecurityEvents")]
        public IActionResult InsertSecurityEvent([FromBody] SecurityEventsInsertUtil newLog)
        {
            var _insertSecurityEventsBLL = new InsertSecurityEventsBLL();
            var result = _insertSecurityEventsBLL.InsertLog(newLog);
            if (result.Item1)
            {
                return Ok("Successfully inserted");
            }
            else
            {
                return BadRequest(result.Item2);
            }
        }

        [HttpPost("SystemLogs")]
        public IActionResult InsertSystemLog([FromBody] SystemLogsInsertUtil newLog)
        {
            var _insertSystemLogsBLL = new InsertSystemLogsBLL();
            var result = _insertSystemLogsBLL.InsertLog(newLog);
            if (result.Item1)
            {
                return Ok("Successfully inserted");
            }
            else
            {
                return BadRequest(result.Item2);
            }
        }

        [HttpPost("UserOpsLogs")]
        public IActionResult InsertUserOpsLog([FromBody] UserOpsLogsInsertUtil newLog)
        {
            var _insertUserOpsLogsBLL = new InsertUserOpsLogsBLL();
            var result = _insertUserOpsLogsBLL.InsertLog(newLog);
            if (result.Item1)
            {
                return Ok("Successfully inserted");
            }
            else
            {
                return BadRequest(result.Item2);
            }
        }

        [HttpPost("NotificationLogs")]
        public IActionResult InsertNotificationLog([FromBody] NotificationLogsInsertUtil newLog)
        {
            var _insertNotificationLogBLL = new InsertNotificationLogBLL();
            var result = _insertNotificationLogBLL.InsertNotificationLog(newLog);
            if (result.Item1)
            {
                return Ok("Successfully inserted");
            }
            else
            {
                return BadRequest(result.Item2);
            }
        }

        [HttpPost("RecommendationLogs")]
        public IActionResult InsertRecommendationLog([FromBody] RecommendationLogsInsertUtil newLog)
        {
            var _insertRecommendationLogBLL = new InsertRecommendationLogBLL();
            var result = _insertRecommendationLogBLL.InsertRecommendationLog(newLog);
            if (result.Item1)
            {
                return Ok("Successfully inserted");
            }
            else
            {
                return BadRequest(result.Item2);
            }
        }
    }
}
