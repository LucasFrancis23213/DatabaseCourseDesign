using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Diagnostics;
using SQLOperation.PublicAccess.Templates.SQLManager;
using Newtonsoft.Json.Linq;
using SQLOperation.PublicAccess.Utilities;
using SQLOperation.BusinessLogicLayer.BasicFeatureBLL;
using SQLOperation.PublicAccess.InterfaceManager.BasicFeatureInterface;

namespace WebAppTest.APILayer.BasicFeatureAPI
{
    [ApiController]
    public class ItemMatchCotroller : ControllerBase
    {
        //private Connection conn;
        //private OracleConnection OracleConnection;
        private ItemMatch ItemMatchObject = new ItemMatch();


        [Route("api/ItemMatch/ClaimAndMatch")]
        [HttpPost]
        public bool MatchItemOPs([FromBody] dynamic InputMatJson)
        {
            try
            {
                string MatString = InputMatJson.ToString();
                JObject TmpJson = JObject.Parse(MatString);
                Item_Claim_Processes ItemClaimProcessObj = new Item_Claim_Processes();
                List<Item_Claim_Processes> ItemClaimProcess = new List<Item_Claim_Processes>();
                Match_Records RecordsObj = new Match_Records();
                List<Match_Records> Records = new List<Match_Records>();
                List<string> status = new List<string>();

                try
                {
                    //由于前端传入的数据涉及多张表，此处手动解析
                    ItemClaimProcessObj.Process_ID = TmpJson["PROCESS_ID"].ToString();
                    ItemClaimProcessObj.Item_ID = TmpJson["ITEM_ID"].ToString();
                    ItemClaimProcessObj.Claimant_User_ID = TmpJson["CLAIMANT_USER_ID"].ToString();
                    ItemClaimProcessObj.Status = TmpJson["STATUS"].ToString();
                    ItemClaimProcessObj.Application_Date = DateTime.Now;
                    //自动匹配部分
                    if (ItemClaimProcessObj.Status == "审核通过")
                    {
                        RecordsObj.Record_ID = TmpJson["RECORD_ID"].ToString();
                        RecordsObj.Lost_Item_ID = ItemClaimProcessObj.Claimant_User_ID;
                        RecordsObj.Found_Item_ID = TmpJson["FOUND_ID"].ToString();
                        RecordsObj.Match_Date = DateTime.Now;
                        RecordsObj.Processing_Status = TmpJson["PROCESSING_STATUS"].ToString();
                    }
                    //这里如果不用null是否会造成后续不对应？
                    else
                    {
                        RecordsObj.Record_ID = null;
                        RecordsObj.Lost_Item_ID = null;
                        RecordsObj.Found_Item_ID = null;
                        RecordsObj.Match_Date = DateTime.Now;
                        RecordsObj.Processing_Status = null;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Json Deserialization Error: {ex.Message}");
                    return false;
                }

                ItemClaimProcess.Add(ItemClaimProcessObj);
                Records.Add(RecordsObj);
                status.Add(ItemClaimProcessObj.Status);
                Tuple<bool, string> OperationStatus = ItemMatchObject.AutoMatch(status, Records);
                return OperationStatus.Item1;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"In MatchItem Function,报错为：{ex.Message}");
                return false;
            }
        }


        [Route("api/ItemMatch/Read")]
        [HttpPost]
        public bool ReadOPs([FromBody] dynamic InputJson)
        {
            try
            {
                string reString = InputJson.ToString();
                JObject TmpJson = JObject.Parse(reString);
                Item_Return_Agreements ReadItemObj = new Item_Return_Agreements();
                try
                {
                    //先进行阅读协议，此处传进来应该不需要内容，否则太大
                    ReadItemObj.Agreement_ID = TmpJson["AGREEMENT_ID"].ToString();
                    ReadItemObj.Item_ID = TmpJson["ITEM_ID"].ToString();
                    ReadItemObj.From_User_ID = TmpJson["FROM_USER_ID"].ToString();
                    ReadItemObj.To_User_ID = TmpJson["TO_USER_ID"].ToString();
                    ReadItemObj.Agreement_Content = "contents";//这里每个人的都一样，有必要给出吗？
                    ReadItemObj.Exchange_Status = TmpJson["EXCHANGE_STATUS"].ToString();
                    ReadItemObj.Creation_Time = DateTime.Now;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Json Deserialization Error: {ex.Message}");
                    return false;
                }
                Tuple<bool, string> OperationStatus = ItemMatchObject.ItemReturnAgreementBasic(ReadItemObj);
                return OperationStatus.Item1;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"In readcontents Function,报错为：{ex.Message}");
                return false;
            }
        }

        [Route("api/ItemMatch/ExchangeItem")]
        [HttpPost]
        public bool ExchangeItemOPs([FromBody] dynamic InputJson)
        {
            try
            {
                string ExchangeItemString = InputJson.ToString();
                JObject TmpJson = JObject.Parse(ExchangeItemString);
                Item_Exchanges ExchangeItemObj = new Item_Exchanges();
                List<Item_Exchanges> ExchangeItems = new List<Item_Exchanges>();
                List<string> statuses = new List<string>();
                string status = "0";
                try
                {
                    status = TmpJson["STATUS"].ToString();
                    ExchangeItemObj.Exchange_ID = TmpJson["EXCHANGE_ID"].ToString();
                    ExchangeItemObj.Lost_Item_ID = TmpJson["LOST_ITEM_ID"].ToString();
                    ExchangeItemObj.Found_Item_ID = TmpJson["FOUND_ITEM_ID"].ToString();
                    ExchangeItemObj.Initiator_User_ID = TmpJson["INITIATOR_USER_ID"].ToString();
                    ExchangeItemObj.Transaction_Type = TmpJson["TRANSACTION_TYPE"].ToString();
                    ExchangeItemObj.Responder_User_ID = TmpJson["RESPONDER_USER_ID"].ToString();
                    ExchangeItemObj.Exchange_Status = TmpJson["EXCHANGE_STATUS"].ToString();
                    ExchangeItemObj.Creation_Time = DateTime.Now;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Json Deserialization Error: {ex.Message}");
                    return false;
                }
                ExchangeItems.Add(ExchangeItemObj);
                statuses.Add(status);
                Tuple<bool, string> OperationStatus = ItemMatchObject.ExchangeItem(statuses,ExchangeItems);
                return OperationStatus.Item1;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"In returnitem Function,报错为：{ex.Message}");
                return false;
            }
        }

        //conditions什么方法传入比较好？
        /*
        [Route("api/ItemMatch/QueryItem")]
        [HttpGet]
        public IActionResult QueryItem([FromQuery] string type, [FromQuery] int review)
        {
            // type==0表格Lost_Item
            // type==1表格Found_Item
            try
            {
                Dictionary<string, object> Conditions = new Dictionary<string, object>();

                // 添加where条件
                if (review == 0)
                    Conditions.Add("Review_Status", "0");
                else if (review == 1)
                    Conditions.Add("Review_Status", "1");

                Tuple<bool, string> OperationStatus = ItemMatch.QueryItem(type, Conditions);

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
                Debug.WriteLine($"In QueryMatchItem Function,报错为：{ex.Message}");
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

                try
                {
                    string DelString = InputDelJson.ToString();
                    JObject TmpJson = JObject.Parse(DelString);
                    string TableName = TmpJson["TABLENAME"].ToString();
                    Conditions.Add("ITEM_ID", TmpJson["ITEM_ID"].ToString());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Json Deserialization Error: {ex.Message}");
                    return BadRequest("Json解析错误");
                }

                Tuple<bool, string> OperationStatus = ItemMatch.DeleteItem(TableName, Conditions);

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
                Debug.WriteLine($"In deletematchItem Function,报错为：{ex.Message}");
                return StatusCode(500, "服务器内部错误");
            }
        }
        */

}
