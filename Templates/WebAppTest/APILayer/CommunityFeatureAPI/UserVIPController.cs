using DatabaseProject.ServiceLayer.ConmmunityFeature;
using Microsoft.AspNetCore.Mvc;
using SQLOperation.PublicAccess.Templates.SQLManager;
using SQLOperation.PublicAccess.Utilities;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Transactions;
using DatabaseProject.BusinessLogicLayer.ServiceLayer.ConmmunityFeature;

namespace WebAppTest.APILayer.CommunityFeatureAPI
{
    [Route("api/vip/")]
    [ApiController]
    public class UserVIPController : Controller
    {
        private UserVIP userVIPService;
        public UserVIPController(Connection connection)
        {
            this.userVIPService = new UserVIP(connection);
        }
        // 判断是否是vip
        [HttpPost("isMember")]
        public IActionResult IsVIPMember([FromBody] Dictionary<string, JsonElement> requestBody)
        {
            try
            {
                int userId = requestBody["user_id"].GetInt32();   
                int vipMemberId = userVIPService.IsVIP(userId);
                bool isVip = vipMemberId > 0;

                return Ok(new
                {
                    status = "success",
                    is_vip = isVip
                });
               
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "error",
                    message = ex.Message
                });
            }


        }

        // 用户充值VIP 测试成功
        [HttpPost("RechargeVIP")]
        public IActionResult RechargeVip([FromBody] Dictionary<string, JsonElement> request)
        {
            try
            {
                if (!request.ContainsKey("user_id") || !request.ContainsKey("recharge_time") || !request.ContainsKey("total_amount"))
                {
                    return BadRequest(new { status = "error", message = "Missing required fields." });
                }
                int userId = request["user_id"].GetInt32();
                int rechargeTime = request["recharge_time"].GetInt32();
                double totalAmount = request["total_amount"].GetDouble();
                if (userId <= 0 || rechargeTime <= 0 || totalAmount <= 0)
                {
                    return BadRequest(new { status = "error", message = "Invalid values." });
                }

                var vipOrder = userVIPService.RechargeVIP(userId, rechargeTime, totalAmount);

                return Ok(new
                {
                    status = "success",
                    order_id = vipOrder.Item1.Order_ID,
                    vip_start_date = vipOrder.Item2,
                    vip_end_date = vipOrder.Item2.AddMonths(rechargeTime),
                    vip_status = "Active",
                    total_amount=totalAmount,
                    point_return=vipOrder.Item1.Point_Return,
                    
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "error",
                    message = ex.Message
                });
            }
        }

        // 获取用户VIP状态
        [HttpPost("GetVIPstatus")]
        public IActionResult GetVIPstatus([FromBody] Dictionary<string, JsonElement> request)
        {
            try
            {
                if (!request.ContainsKey("user_id"))
                {
                    return BadRequest(new { status = "error", message = "Missing user_id field." });
                }

                int userId = request["user_id"].GetInt32();

                if (userId <= 0)
                {
                    return BadRequest(new { status = "error", message = "Invalid user_id." });
                }

                var vipInfo = userVIPService.GetVIPInfo(userId);


                var firstVipInfo = vipInfo.Select(v => new
                {
                    vip_member_id = v.VIP_Member_ID,
                    vip_start_time = v.VIP_Start_Date.ToLocalTime(),
                    vip_end_time = v.VIP_End_Date.ToLocalTime(),
                    vip_status = v.Status
                }).FirstOrDefault();

                return Ok(new
                {
                    status = "success",
                    vip_info = firstVipInfo ?? null
                });


            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "error",
                    message = ex.Message
                });
            }
        }

        // 增加vipmember 测试成功
        [HttpPost("AddVIPMember")]
        public IActionResult AddVIPMember([FromBody] Dictionary<string, JsonElement> request)
        {
            try
            {
                
                if (!request.ContainsKey("user_id") || !request.ContainsKey("start_time")||
                    !request.ContainsKey("end_time") || !request.ContainsKey("vip_status"))
                {
                    return BadRequest(new { status = "error", message = "Missing required fields." });
                }

                int userId = request["user_id"].GetInt32();
                DateTime vipStartDate = request["start_time"].GetDateTime();
                DateTime vipEndDate = request["end_time"].GetDateTime();
                string vipStatus = request["vip_status"].GetString();

                if (userId <= 0 || string.IsNullOrEmpty(vipStatus))
                {
                    return BadRequest(new { status = "error", message = "Invalid values." });
                }

                int vipMemberId = userVIPService.AddVIPMember(userId, vipStartDate, vipEndDate, vipStatus);

                return Ok(new
                {
                    status = "success",
                    vip_member_id = vipMemberId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "error",
                    message = ex.Message
                });
            }
        }

        // 删除vipmember 测试成功
        [HttpDelete("DeleteVIPMember")]
        public IActionResult DeleteVIPMember([FromBody] Dictionary<string, JsonElement> request)
        {
            try
            {
                if (!request.ContainsKey("vip_member_id"))
                {
                    return BadRequest(new { status = "error", message = "Missing vip_member_id field." });
                }

                int vipMemberId = request["vip_member_id"].GetInt32();

                if (vipMemberId <= 0)
                {
                    return BadRequest(new { status = "error", message = "Invalid vip_member_id." });
                }

                bool result = userVIPService.DeleteVIPMember(vipMemberId);

                if (result)
                {
                    return Ok(new { status = "success" });
                }
                else
                {
                    return NotFound(new { status = "error", message = "VIP member not found." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "error",
                    message = ex.Message
                });
            }
        }

        // 更新vipmember 测试成功
        [HttpPut("UpdateVIPMember")]
        public IActionResult UpdateVIPMember([FromBody] Dictionary<string, JsonElement> request)
        {
            try
            {
                if (!request.ContainsKey("vip_member_id"))
                {
                    return BadRequest(new { status = "error", message = "Missing vip_member_id field." });
                }

                int vipMemberId = request["vip_member_id"].GetInt32();
                if (vipMemberId <= 0)
                {
                    return BadRequest(new { status = "error", message = "Invalid vip_member_id." });
                }

                var updateParams = new Dictionary<string, object>();

                if (request.ContainsKey("user_id"))
                {
                    updateParams["user_id"] = request["user_id"].GetInt32();
                }
                if (request.ContainsKey("start_time"))
                {
                    return BadRequest(new { status = "error", message = "错误，不能修改起始时间" });
                }
                if (request.ContainsKey("end_time"))
                {
                    updateParams["vip_end_date"] = request["end_time"].GetDateTime();
                }
                if (request.ContainsKey("vip_status"))
                {
                    updateParams["status"] = request["vip_status"].GetString();
                }

                bool result = userVIPService.UpdateVIPMember(vipMemberId, updateParams);

                if (result)
                {
                    return Ok(new { status = "success" });
                }
                else
                {
                    return NotFound(new { status = "error", message = "VIP member not found or update failed." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "error",
                    message = ex.Message
                });
            }
        }

    }


}



