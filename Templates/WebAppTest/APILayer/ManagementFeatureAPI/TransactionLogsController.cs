using Microsoft.AspNetCore.Mvc;
using SQLOperation.BusinessLogicLayer.ManagementFeatureBLL;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

namespace WebAppTest.APILayer.ManagementFeatureAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionLogsController : ControllerBase
    {
        [HttpGet("GetTransactionLogs")]
        public IActionResult GetTransactionLogs([FromQuery] QueryTransactionLogsArgs args)
        {
            GetTransactionLogsBLL _getTransactionLogsBLL = new();
            var result = _getTransactionLogsBLL.GetLogs(args);
            if (result.Item1)
            {
                return Ok(result.Item2);
            }
            else
            {
                return BadRequest(result.Item2);
            }
        }

        [HttpPost("InsertTransactionLog")]
        public IActionResult InsertTransactionLog([FromBody] TransactionLogsInsertUtil newLog)
        {
            InsertTransactionLogBLL _insertTransactionLogBLL = new();
            var result = _insertTransactionLogBLL.InsertTransactionLog(newLog);
            if (result.Item1)
            {
                return Ok(result.Item2);
            }
            else
            {
                return BadRequest(result.Item2);
            }
        }

        [HttpPut("UpdateTransactionStatus")]
        public IActionResult ChangeTransactionStatus(int TransactionID, string newStatus, DateTime? FinishTime)
        {
            UpdateTransactionStatusBLL _updateTransactionStatusBLL = new();
            var result = _updateTransactionStatusBLL.UpdateTransactionStatus(TransactionID, newStatus, FinishTime);
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
