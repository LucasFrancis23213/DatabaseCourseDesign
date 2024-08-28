using Microsoft.AspNetCore.Mvc;
using SQLOperation.BusinessLogicLayer.ManagementFeatureBLL;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

namespace WebAppTest.APILayer.ManagementFeatureAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionLogsController : ControllerBase
    {
        private readonly InsertTransactionLogBLL _insertTransactionLogBLL;
        private readonly GetTransactionLogsBLL _getTransactionLogsBLL;
        private readonly UpdateTransactionStatusBLL _updateTransactionStatusBLL;
        public TransactionLogsController()
        {
            _getTransactionLogsBLL = new GetTransactionLogsBLL();
            _insertTransactionLogBLL = new InsertTransactionLogBLL();
            _updateTransactionStatusBLL = new UpdateTransactionStatusBLL();
        }

        [HttpGet("GetTransactionLogs")]
        public IActionResult GetTransactionLogs([FromQuery] QueryTransactionLogsArgs args)
        {
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
