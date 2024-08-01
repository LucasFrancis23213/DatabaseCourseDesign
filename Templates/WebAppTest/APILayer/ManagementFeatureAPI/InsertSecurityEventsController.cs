using Microsoft.AspNetCore.Mvc;
using SQLOperation.BusinessLogicLayer.ManagementFeatureBLL;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

namespace WebAppTest.APILayer.ManagementFeatureAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class InsertSecurityEventsController : ControllerBase
    {
        private InsertSecurityEventsBLL InsertSecurityEventsBLL;
        public InsertSecurityEventsController()
        {
            InsertSecurityEventsBLL = new();
        }

        [HttpPost]
        public IActionResult InsertSecurityEvent([FromBody] SecurityEventsInsertUtil NewLog)
        {
            var result = InsertSecurityEventsBLL.InsertLog(NewLog);

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
