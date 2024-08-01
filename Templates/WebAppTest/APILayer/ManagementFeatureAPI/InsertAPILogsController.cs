using Microsoft.AspNetCore.Mvc;
using SQLOperation.BusinessLogicLayer.ManagementFeatureBLL;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

namespace WebAppTest.APILayer.ManagementFeatureAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class InsertAPILogsController : ControllerBase
    {
        private InsertAPILogsBLL InsertAPILogsBLL;
        public InsertAPILogsController()
        {
            InsertAPILogsBLL = new();
        }

        [HttpPost]
        public IActionResult InsertAPILog([FromBody] APIAccessLogsInsertUtil NewLog)
        {
            var result = InsertAPILogsBLL.InsertLog(NewLog);

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

