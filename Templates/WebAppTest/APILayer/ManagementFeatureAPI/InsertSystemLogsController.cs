using Microsoft.AspNetCore.Mvc;
using SQLOperation.BusinessLogicLayer.ManagementFeatureBLL;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

namespace WebAppTest.APILayer.ManagementFeatureAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class InsertSystemLogsController : ControllerBase
    {
        private InsertSystemLogsBLL InsertSystemLogsBLL;
        public InsertSystemLogsController()
        {
            InsertSystemLogsBLL = new();
        }

        [HttpPost]
        public IActionResult InsertSystemLog([FromBody] SystemLogsInsertUtil NewLog)
        {
            var result = InsertSystemLogsBLL.InsertLog(NewLog);

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