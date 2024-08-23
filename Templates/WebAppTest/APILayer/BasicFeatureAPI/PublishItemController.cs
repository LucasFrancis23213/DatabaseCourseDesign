using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Diagnostics;
using SQLOperation.PublicAccess.Templates.SQLManager;
using Newtonsoft.Json.Linq;
using SQLOperation.PublicAccess.Utilities;
using SQLOperation.BusinessLogicLayer.BasicFeatureBLL;

namespace WebAppTest.APILayer.BasicFeatureAPI
{
    [ApiController]
    public class PublishItemController : ControllerBase
    {
        //private Connection conn;
        //private OracleConnection OracleConnection;
        private PublishItem PublishItemObject = new PublishItem();


        [Route("api/PublishItem/Lost")]
        [HttpPost]
        public bool LostItemOPs([FromBody] dynamic InputPubJson)
        {
            try
            {
                string PubString = InputPubJson.ToString();
                JObject TmpJson = JObject.Parse(PubString);
                Lost_Item LostItemObj = new Lost_Item();
                List<Lost_Item> LostItems = new List<Lost_Item>();
                Reward_Offers RewardObj = new Reward_Offers();
                List<Reward_Offers> Rewards = new List<Reward_Offers>();

                try
                {
                    //由于前端传入的数据涉及多张表，此处手动解析
                    LostItemObj.Item_ID = TmpJson["ITEM_ID"].ToString();
                    LostItemObj.Item_Name = TmpJson["ITEM_NAME"].ToString();
                    LostItemObj.Category_ID = (int)TmpJson["CATEGORY_ID"];
                    LostItemObj.Description = TmpJson["DESCRIPTION"].ToString();
                    LostItemObj.Lost_Location = TmpJson["LOST_LOCATION"].ToString();
                    LostItemObj.Lost_Date = (DateTime)TmpJson["LOST_DATE"];

                    //LostItemObj.User_ID = (int)TmpJson["User_ID"];
                    LostItemObj.User_ID = 65; // Debug Only

                    LostItemObj.Lost_Status = "LOST";
                    LostItemObj.Review_Status = 0; // 默认是Pending
                    LostItemObj.Image_URL = TmpJson["IMAGE_URL"].ToString();
                    LostItemObj.Tag_ID = (int)TmpJson["TAG_ID"];
                    LostItemObj.Is_Rewarded = (int)TmpJson["IS_REWARDED"];

                    //再处理悬赏部分
                    if ((bool)TmpJson["IS_REWARDED"])
                    {
                        RewardObj.Deadline = (DateTime)TmpJson["DEADLINE"];
                        //Release_Date自动为调用函数时候的时间
                        RewardObj.Release_Date = DateTime.Now;
                        RewardObj.Reward_Amount = (int)TmpJson["REWARD_AMOUNT"];
                        RewardObj.Status = "LOST";
                        RewardObj.Item_ID = LostItemObj.Item_ID;
                        RewardObj.User_ID = LostItemObj.User_ID;
                    }
                    else
                    {
                        RewardObj.Item_ID = LostItemObj.Item_ID;
                        RewardObj.User_ID = LostItemObj.User_ID;
                    }


                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Json Deserialization Error: {ex.Message}");
                    return false;
                }

                LostItems.Add(LostItemObj);
                Rewards.Add(RewardObj);
                Tuple<bool, string> OperationStatus = PublishItemObject.PublishLostItem(LostItems, Rewards);
                return OperationStatus.Item1;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"In PublishLostItem Function,报错为：{ex.Message}");
                return false;
            }
        }


        [Route("api/PublishItem/Found")]
        [HttpPost]
        public bool FoundItemOPs([FromBody] dynamic InputPubJson)
        {
            try
            {
                string PubString = InputPubJson.ToString();
                JObject TmpJson = JObject.Parse(PubString);
                Found_Item FoundItemObj = new Found_Item();
                List<Found_Item> FoundItems = new List<Found_Item>();

                try
                {
                    //由于前端传入的数据涉及多张表，此处手动解析
                    FoundItemObj.Item_ID = TmpJson["ITEM_ID"].ToString();
                    FoundItemObj.Item_Name = TmpJson["ITEM_NAME"].ToString();
                    FoundItemObj.Category_ID = (int)TmpJson["CATEGORY_ID"];
                    FoundItemObj.Description = TmpJson["DESCRIPTION"].ToString();
                    FoundItemObj.Found_Location = TmpJson["FOUND_LOCATION"].ToString();
                    FoundItemObj.Found_Date = (DateTime)TmpJson["FOUND_DATE"];

                    //FoundItemObj.User_ID = (int)TmpJson["User_ID"];
                    FoundItemObj.User_ID = 65; // Debug Only

                    FoundItemObj.Match_Status = "Matching";
                    FoundItemObj.Review_Status = 0; // 默认是Pending
                    FoundItemObj.Image_URL = TmpJson["IMAGE_URL"].ToString();
                    FoundItemObj.Tag_ID = (int)TmpJson["TAG_ID"];

                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Json Deserialization Error: {ex.Message}");
                    return false;
                }

                FoundItems.Add(FoundItemObj);
                Tuple<bool, string> OperationStatus = PublishItemObject.PublishFoundItem(FoundItems);
                return OperationStatus.Item1;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"In PublishFoundItem Function,报错为：{ex.Message}");
                return false;
            }
        }

        [Route("api/QueryItem")]
        [HttpGet]
        public IActionResult QueryItem([FromQuery] int type, [FromQuery] int review)
        {
            // type==0表格Lost_Item
            // type==1表格Found_Item
            try
            {
                Dictionary<string, object> Conditions = new Dictionary<string, object>();

                // 添加一条where条件
                if (review == 0)
                    Conditions.Add("Review_Status", "0");
                else if (review == 1)
                    Conditions.Add("Review_Status", "1");

                Tuple<bool, string> OperationStatus = PublishItemObject.QueryItem(type, Conditions);

                if (OperationStatus.Item1)
                {
                    // OperationStatus.Item2 包含 JSON 格式的数据
                    return Ok(OperationStatus.Item2);
                }
                else
                {
                    return BadRequest("查询失败");
                }


            }
            catch (Exception ex)
            {
                Debug.WriteLine($"In QueryLostItem Function,报错为：{ex.Message}");
                return StatusCode(500, "服务器内部错误");
            }
        }

        [Route("api/DeleteItem")]
        [HttpDelete]
        public IActionResult DeleteItem([FromBody] dynamic InputDelJson)
        {
            // type==0表格Lost_Item
            // type==1表格Found_Item
            try
            {
                Dictionary<string, object> Conditions = new Dictionary<string, object>();

                int type = 0;

                try
                {
                    string DelString = InputDelJson.ToString();
                    JObject TmpJson = JObject.Parse(DelString);
                    type = (int)TmpJson["type"];
                    Conditions.Add("ITEM_ID", TmpJson["ITEM_ID"].ToString());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Json Deserialization Error: {ex.Message}");
                    return BadRequest("Json解析错误");
                }

                Tuple<bool, string> OperationStatus = PublishItemObject.DeleteItem(type, Conditions);

                if (OperationStatus.Item1)
                {
                    // OperationStatus.Item2 包含 JSON 格式的数据
                    return Ok(OperationStatus.Item2);
                }
                else
                {
                    return BadRequest("删除失败");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"In QueryLostItem Function,报错为：{ex.Message}");
                return StatusCode(500, "服务器内部错误");
            }
        }

        [Route("api/PassItem")]
        [HttpPost]
        public IActionResult ReviewItem([FromBody] dynamic InputRevJson)
        {
            // type==0表格Lost_Item
            // type==1表格Found_Item
            try
            {
                List<string> Item_IDs = new List<string>();

                int type = 0;

                try
                {
                    string RevString = InputRevJson.ToString();
                    JObject TmpJson = JObject.Parse(RevString);
                    type = (int)TmpJson["type"];
                    Item_IDs.Add(TmpJson["ITEM_ID"].ToString());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Json Deserialization Error: {ex.Message}");
                    return BadRequest("Json解析错误");
                }

                Tuple<bool, string> OperationStatus = PublishItemObject.ReviewItem(type, Item_IDs);

                if (OperationStatus.Item1)
                {
                    // OperationStatus.Item2 包含 JSON 格式的数据
                    return Ok(OperationStatus.Item2);
                }
                else
                {
                    return BadRequest("审核失败");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"In QueryLostItem Function,报错为：{ex.Message}");
                return StatusCode(500, "服务器内部错误");
            }
        }
    }

}