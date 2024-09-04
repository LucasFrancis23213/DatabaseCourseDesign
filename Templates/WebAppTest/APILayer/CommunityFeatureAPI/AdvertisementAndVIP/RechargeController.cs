using DatabaseProject.APILayer.CommunityFeatureAPI;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebAppTest.APILayer.CommunityFeatureAPI;


namespace WebAppTest.APILayer.CommunityFeatureAPI.AdvertisementAndVIP
{
    [ApiController]
    [Route("api/recharge/")]
    public class RechargeController : ControllerBase
    {
       
        private readonly RechargeWebsocketService _webSocketService; // 注入 WebSocketService
        public RechargeController(RechargeWebsocketService webSocketService)
        {
            _webSocketService = webSocketService;
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> Recharge([FromBody] RechargeRequest request)
        {
            // 这里调用异步的充值方法
            bool rechargeSuccess = await SimulateRechargeAsync(request.UserId);

            // 向前端发送充值结果的消息
            await _webSocketService.SendMessageAsync(request.UserId, rechargeSuccess ? "Recharge Success" : "Recharge Failed");

            return Ok(new { Success = rechargeSuccess });
        }

        private async Task<bool> SimulateRechargeAsync(int userId)
        {
            // 模拟充值操作
            await Task.Delay(5000);

            return true; // 假设充值成功
        }
    }

    public class RechargeRequest
    {
        public int UserId { get; set; }
    }
}

