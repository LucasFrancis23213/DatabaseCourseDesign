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
    [Route("api/advertisement/")]
    [ApiController]
    public class AdvertisementsController : Controller
    {

        private AdvertisementsService advertisementsService;
        public AdvertisementsController(Connection connection)
        {
            advertisementsService = new AdvertisementsService(connection);
        }

        // 用户随机呈现广告 测试通过
        [HttpPost("GetRandomAd")]
        public IActionResult GetRandomAd([FromBody] Dictionary<string, JsonElement> request)
        {
            try
            {
                // 检查是否包含 "user_id" 键
                if (!request.ContainsKey("user_id"))
                {
                    var errorResponse = new
                    {
                        status = "error",
                        message = "请求中缺少 user_id 参数"
                    };
                    return BadRequest(errorResponse);
                }

                int userId = request["user_id"].GetInt32();
                var ad = advertisementsService.GetAdForUser(userId);
                var response = new
                {
                    status = "success",
                    ad = new
                    {
                        id = ad.Ad_ID,
                        content = ad.Ad_Content,
                        picture = ad.Ad_Picture,
                        url = ad.Ad_URL,
                        type = ad.Ad_Type
                    }
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = new
                {
                    status = "error",
                    message = $"内部服务器错误：{ex.Message}"
                };
                return StatusCode(500, errorResponse);
            }
        }


        // 用户点击广告 测试成功
        [HttpPost("ClickAd")]
        public IActionResult ClickAd([FromBody] Dictionary<string, JsonElement> request)
        {
            try
            {
                // 检查是否包含必要的键
                if (!request.ContainsKey("user_id") ||
                    !request.ContainsKey("ad_id") ||
                    !request.ContainsKey("click_time"))
                {
                    return BadRequest(new { status = "error", message = "请求中缺少必要参数" });
                }

                int userId = request["user_id"].GetInt32();
                int adId = request["ad_id"].GetInt32();
                DateTime clickTime = request["click_time"].GetDateTime();
                string ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();

                var success = advertisementsService.UserClickAd(userId, adId, clickTime, ipAddress);

                if (success)
                {
                    return Ok(new { status = "success" });
                }
                else
                {
                    return StatusCode(500, new { status = "error", message = "记录广告点击时发生错误" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = $"内部服务器错误: {ex.Message}" });
            }
        }

        // 增加广告 测试通过
        [HttpPost("AddAdvertisement")]
        public IActionResult AddAdvertisement([FromBody] Dictionary<string, JsonElement> request)
        {
            try
            {
                // 检查是否包含必要的键
                if (!request.ContainsKey("ad_content") ||
                    !request.ContainsKey("ad_picture") ||
                    !request.ContainsKey("ad_url") ||
                    !request.ContainsKey("ad_type") ||
                    !request.ContainsKey("start_time") ||
                    !request.ContainsKey("end_time"))
                {
                    return BadRequest(new { status = "error", message = "请求中缺少必要参数" });
                }

                // 获取并验证
                string content = ControllerHelper.GetSafeString(request, "ad_content");
                string picture = ControllerHelper.GetSafeString(request, "ad_picture");
                string url = ControllerHelper.GetSafeString(request, "ad_url");
                string type = ControllerHelper.GetSafeString(request, "ad_type");
                // 获取
                DateTime startTime = request["start_time"].GetDateTime();
                DateTime endTime = request["end_time"].GetDateTime();

                if (startTime >= endTime)
                {
                    throw new Exception("非法时间错误");
                }

                int adId = advertisementsService.AddAdvertisement(content, picture, url, type, startTime, endTime);

                return Ok(new { status = "success", ad_id = adId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = $"内部服务器错误: {ex.Message}" });
            }
        }

        // 更新广告 不能更新不存在的广告 测试通过
        [HttpPut("UpdateAdvertisement")]
        public IActionResult UpdateAdvertisement([FromBody] Dictionary<string, JsonElement> request)
        {
            try
            {
                int adId = request["ad_id"].GetInt32();
                var updateFields = new Dictionary<string, object>();

                if (request.ContainsKey("ad_content"))
                    updateFields["ad_content"] = ControllerHelper.GetSafeString(request, "ad_content");

                if (request.ContainsKey("ad_picture"))
                    updateFields["ad_picture"] = ControllerHelper.GetSafeString(request, "ad_picture");

                if (request.ContainsKey("ad_url"))
                    updateFields["ad_url"] = ControllerHelper.GetSafeString(request, "ad_url");

                if (request.ContainsKey("ad_type"))
                    updateFields["ad_type"] = ControllerHelper.GetSafeString(request, "ad_type");

                if (request.ContainsKey("start_time"))
                    updateFields["start_time"] = request["start_time"].GetDateTime();
                if (request.ContainsKey("end_time"))
                    updateFields["end_time"] = request["end_time"].GetDateTime();

                var success = advertisementsService.UpdateAdvertisement(adId, updateFields);

                if (success)
                {
                    return Ok(new { status = "success" });
                }
                else
                {
                    return StatusCode(500, new { status = "error", message = "更新广告时发生错误" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = $"内部服务器错误: {ex.Message}" });
            }
        }

        // 删除广告 测试成功 不能删除不存在的广告
        [HttpDelete("DeleteAdvertisement")]
        public IActionResult DeleteAdvertisement([FromBody] Dictionary<string, JsonElement> request)
        {
            try
            {
                // 检查是否包含 "ad_id" 键
                if (!request.ContainsKey("ad_id"))
                {
                    return BadRequest(new { status = "error", message = "请求中缺少 ad_id 参数" });
                }

                int adId = request["ad_id"].GetInt32();

                var success = advertisementsService.RemoveAdvertisement(adId);

                if (success)
                {
                    return Ok(new { status = "success" });
                }
                else
                {
                    return StatusCode(500, new { status = "error", message = "删除广告时发生错误" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = $"内部服务器错误: {ex.Message}" });
            }
        }

        // 获取所有广告列表 测试成功 包括空表和有广告
        [HttpPost("GetAdvertisements")]
        public IActionResult GetAdvertisements()
        {
            try
            {
                var ads = advertisementsService.GetAdvertisementInfo();

                var response = new
                {
                    status = "success",
                    advertisements = ads.Select(ad => new
                    {
                        ad_id = ad.Ad_ID,
                        ad_content = ad.Ad_Content,
                        ad_picture = ad.Ad_Picture,
                        ad_url = ad.Ad_URL,
                        ad_type = ad.Ad_Type,
                        start_time = ad.Start_Time,
                        end_time = ad.End_Time
                    }).ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = $"内部服务器错误： {ex.Message}" });
            }
        }

        // 获取广告具体细节 测试通过
        [HttpPost("GetAdvertisementDetails")]
        public IActionResult GetAdvertisementDetails([FromBody] Dictionary<string, JsonElement> requestBody)
        {
            try
            {
                if (!requestBody.ContainsKey("ad_id"))
                {
                    return BadRequest(new { status = "error", message = "缺少 ad_id 参数" });
                }

                int adId = requestBody["ad_id"].GetInt32();
                var adDetails = advertisementsService.GetAdDetails(adId);
                var adClickDetails = advertisementsService. GetAdClickDetails(adId);
                return Ok(new
                {
                    status = "success",
                    ad = new
                    {
                        id = adDetails.Ad_ID,
                        content = adDetails.Ad_Content,
                        picture = adDetails.Ad_Picture,
                        url = adDetails.Ad_URL,
                        type = adDetails.Ad_Type,
                        start_time = adDetails.Start_Time.ToLocalTime(),
                        end_time = adDetails.End_Time.ToLocalTime(),
                        click_count = adDetails.Click_Count,
                        show_count = adDetails.Show_Count
                    },
                    click_statistics = adClickDetails.Select(click => new
                    {
                        user_id = click.User_ID,
                        click_time = click.Click_Time.ToLocalTime(),
                        ip_address = click.IP_Address
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                return Ok(new { status = "error", message = $"内部服务器错误: {ex.Message}" });
            }
        }
    }
}
