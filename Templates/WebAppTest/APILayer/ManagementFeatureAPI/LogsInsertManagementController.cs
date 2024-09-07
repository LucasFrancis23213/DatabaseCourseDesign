using Microsoft.AspNetCore.Mvc;
using SQLOperation.BusinessLogicLayer.ManagementFeatureBLL;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

namespace WebAppTest.APILayer.ManagementFeatureAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsInsertController : ControllerBase
    {
        private readonly InsertAPILogsBLL _insertAPILogsBLL;
        private readonly InsertSecurityEventsBLL _insertSecurityEventsBLL;
        private readonly InsertSystemLogsBLL _insertSystemLogsBLL;
        private readonly InsertUserOpsLogsBLL _insertUserOpsLogsBLL;
        private readonly InsertNotificationLogBLL _insertNotificationLogBLL;
        private readonly InsertRecommendationLogBLL _insertRecommendationLogBLL;

        public LogsInsertController()
        {
            _insertAPILogsBLL = new InsertAPILogsBLL();
            _insertSecurityEventsBLL = new InsertSecurityEventsBLL();
            _insertSystemLogsBLL = new InsertSystemLogsBLL();
            _insertUserOpsLogsBLL = new InsertUserOpsLogsBLL();
            _insertNotificationLogBLL = new InsertNotificationLogBLL();
            _insertRecommendationLogBLL = new InsertRecommendationLogBLL();
        }

        [HttpPost("APILogs")]
        public IActionResult InsertAPILog([FromBody] APIAccessLogsInsertUtil newLog)
        {
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
