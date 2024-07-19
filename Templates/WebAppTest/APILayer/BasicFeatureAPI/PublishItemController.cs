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
                    LostItemObj.Item_ID = TmpJson["Item_ID"].ToString();
                    LostItemObj.Item_Name = TmpJson["Item_Name"].ToString();
                    LostItemObj.Category_ID = (int)TmpJson["Category_ID"];
                    LostItemObj.Description = TmpJson["Description"].ToString();
                    LostItemObj.Lost_Location = TmpJson["Lost_Location"].ToString();
                    LostItemObj.Lost_Date = (DateTime)TmpJson["Lost_Date"];
                    
                    //LostItemObj.User_ID = (int)TmpJson["User_ID"];
                    LostItemObj.User_ID = 65; // Debug Only
                    
                    LostItemObj.Lost_Status = "LOST";
                    LostItemObj.Review_Status = 0; // 默认是Pending
                    LostItemObj.Image_URL = TmpJson["Image_URL"].ToString();
                    LostItemObj.Tag_ID = (int)TmpJson["Tag_ID"];

                    //再处理悬赏部分
                    if ((bool)TmpJson["isRewarded"])
                    {
                        RewardObj.Deadline = (DateTime)TmpJson["Deadline"];
                        //Release_Date自动为调用函数时候的时间
                        RewardObj.Release_Date = DateTime.Now;
                        RewardObj.Reward_Amount = (int)TmpJson["Reward_Amount"];
                        RewardObj.Status = "LOST";
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
                Tuple<bool, string> OperationStatus = PublishItemObject.PublishLostItem(LostItems, Rewards, (bool)TmpJson["isRewarded"]);
                return OperationStatus.Item1;
               
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"In PublishItem Function,报错为：{ex.Message}");
                return false;
            }
        }
    }
}
