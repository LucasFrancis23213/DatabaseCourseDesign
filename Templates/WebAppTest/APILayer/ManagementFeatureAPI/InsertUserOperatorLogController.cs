using Microsoft.AspNetCore.Mvc;
using SQLOperation.BusinessLogicLayer.ManagementFeatureBLL;
using SQLOperation.PublicAccess.Utilities;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

namespace WebAppTest.APILayer.ManagementFeatureAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class InsertUserOperatorLogController : ControllerBase
    {
        private InsertUserOpsLogsBLL InsertUserOpsLogsBLL;
        public InsertUserOperatorLogController()
        {
            InsertUserOpsLogsBLL = new();
        }

        [HttpPost]
        public IActionResult InsertUserOpsLog([FromBody] UserOpsLogsInsertUtil NewLog)
        {
            var result = InsertUserOpsLogsBLL.InsertLog(NewLog);

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