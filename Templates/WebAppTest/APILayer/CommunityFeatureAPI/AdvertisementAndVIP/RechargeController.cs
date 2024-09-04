using DatabaseProject.APILayer.CommunityFeatureAPI;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
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
        public async Task<IActionResult> Recharge([FromBody] Dictionary<string,JsonElement> request)
        {
            if (!request.ContainsKey("recharge_id"))
            {
                var errorResponse = new
                {
                    status = "error",
                    message = "请求中缺少 recharge_id 参数"
                };
                return BadRequest(errorResponse);
            }
            string recharge_id = ControllerHelper.GetSafeString(request, "recharge_id");
            
            // 这里调用异步的充值方法
            bool rechargeSuccess =  SimulateRechargeAsync(recharge_id);

            // 向前端发送充值结果的消息
            await _webSocketService.SendMessageAsync(recharge_id, rechargeSuccess ? "Recharge Success" : "Recharge Failed");

            return Ok(new { Success = rechargeSuccess });
        }

        // 充值操作
        private bool SimulateRechargeAsync(string recharge_id)
        {
            // 模拟充值操作

            return true; // 假设充值成功
        }
    }

    
}

