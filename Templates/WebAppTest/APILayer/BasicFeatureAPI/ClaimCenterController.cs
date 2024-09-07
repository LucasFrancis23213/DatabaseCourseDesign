using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Diagnostics;
using SQLOperation.PublicAccess.Templates.SQLManager;
using Newtonsoft.Json.Linq;
using SQLOperation.PublicAccess.Utilities;
using SQLOperation.BusinessLogicLayer.BasicFeatureBLL;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace WebAppTest.APILayer.BasicFeatureAPI
{
    [Route("api/claim/")]
    [ApiController]
    public class ClaimCenterController : ControllerBase
    {
        //private ClaimCenter ItemOpObj = new ClaimCenter();

        [Route("ReturnItem")]
        [HttpPost]
        public IActionResult ReturnItemOPs([FromBody] dynamic InputRtnJson)
        {
            
            try
            {
                string Item_ID = string.Empty;
                Item_Claim_Processes item_Claim_ProcessesObj = new Item_Claim_Processes();
                ItemMatch itemMatchObj = new ItemMatch();
                ClaimCenter ItemOpObj = new ClaimCenter();

                try
                {
                    string RtnString = InputRtnJson.ToString();
                    JObject TmpJson = JObject.Parse(RtnString);
                    Item_ID = TmpJson["ITEM_ID"].ToString();
                    item_Claim_ProcessesObj.Item_ID = Item_ID;
                    item_Claim_ProcessesObj.Status = "WORKING";
                    item_Claim_ProcessesObj.Process_ID = TmpJson["Process_ID"].ToString();
                    item_Claim_ProcessesObj.Claimant_User_ID = (int)TmpJson["Claimant_User_ID"];
                    item_Claim_ProcessesObj.Publish_User_ID = (int)TmpJson["Publish_User_ID"];
                    item_Claim_ProcessesObj.Application_Date = DateTime.Now;

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return BadRequest(ex.Message);
                }

                Tuple<bool, string> AddOperationStatus;
                Tuple<bool, string> ClaimProcessOperationStatus;
                {

                    AddOperationStatus = ItemOpObj.AddReturnItem(Item_ID);
                    ClaimProcessOperationStatus = itemMatchObj.ItemClaimProcessBasic(item_Claim_ProcessesObj);
                    ItemOpObj.ReleaseSQLConn();
                    itemMatchObj.ReleaseSQLConn();
                }


                if (AddOperationStatus.Item1 && ClaimProcessOperationStatus.Item1)
                {
                    // OperationStatus.Item2 包含 JSON 格式的数据
                    return Ok(AddOperationStatus.Item2);
                }
                else
                {
                    return BadRequest("归还失败");
                }

            }
            catch (Exception ex)
            {
               Debug.WriteLine(ex.Message);
               return BadRequest(ex.Message);
            }
        }

        [Route("ClaimItem")]
        [HttpPost]
        public IActionResult ClaimItemOPs([FromBody] dynamic InputClmJson)
        {
            try
            {
                string Item_ID = string.Empty;
                Item_Claim_Processes item_Claim_ProcessesObj = new Item_Claim_Processes();
                ItemMatch itemMatchObj = new ItemMatch();
                ClaimCenter ItemOpObj = new ClaimCenter();
                try
                {
                    string ClmString = InputClmJson.ToString();
                    JObject TmpJson = JObject.Parse(ClmString);
                    Item_ID = TmpJson["ITEM_ID"].ToString();
                    item_Claim_ProcessesObj.Item_ID = Item_ID;
                    item_Claim_ProcessesObj.Status = "WORKING";
                    item_Claim_ProcessesObj.Process_ID = TmpJson["Process_ID"].ToString();
                    item_Claim_ProcessesObj.Claimant_User_ID = (int)TmpJson["Claimant_User_ID"];
                    item_Claim_ProcessesObj.Publish_User_ID = (int)TmpJson["Publish_User_ID"];
                    item_Claim_ProcessesObj.Application_Date = DateTime.Now;

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return BadRequest(ex.Message);
                }

                Tuple<bool, string> AddOperationStatus;
                Tuple<bool, string> ClaimProcessOperationStatus;
                {
                    AddOperationStatus = ItemOpObj.AddClaimItem(Item_ID);
                    ClaimProcessOperationStatus = itemMatchObj.ItemClaimProcessBasic(item_Claim_ProcessesObj);
                    ItemOpObj.ReleaseSQLConn();
                    itemMatchObj.ReleaseSQLConn();
                }

                if (AddOperationStatus.Item1 && ClaimProcessOperationStatus.Item1)
                {
                    // OperationStatus.Item2 包含 JSON 格式的数据
                    return Ok(AddOperationStatus.Item2);
                }
                else
                {
                    return BadRequest("归还失败");
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [Route("QueryItem")]
        [HttpGet]
        public IActionResult QueryItem([FromQuery] int type, [FromQuery] int userID)
        {
            // type==0表格Lost_Item
            // type==1表格Found_Item
           
            try
            {
                Dictionary<string, object> Conditions = new Dictionary<string, object>();
                ClaimCenter ItemOpObj = new ClaimCenter();

                // 添加一条where条件
                Conditions.Add("CLAIMANT_USER_ID", userID);
                Conditions.Add("PUBLISH_USER_ID", userID);

                Tuple<bool, string> OperationStatus;
                {
                    OperationStatus = ItemOpObj.QueryItem(type, Conditions);
                    ItemOpObj.ReleaseSQLConn();
                }

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

        [Route("SignAgreement")]
        [HttpPost]
        public IActionResult SignAgreement([FromBody] dynamic inputSgnJson)
        {
            try
            {
                string JsonString = inputSgnJson.ToString();
                JObject TmpJson = JObject.Parse(JsonString);
                string itemID = TmpJson["itemID"].ToString();
                int userID = (int)TmpJson["userID"];
                ClaimCenter ItemOpObj = new ClaimCenter();

                if (userID > 0)
                {
                    Tuple<bool, string> OperationStatus;
                    {
                        OperationStatus = ItemOpObj.SignReturnAgreement(itemID, userID);
                        ItemOpObj.ReleaseSQLConn();
                    }
                    if (OperationStatus.Item1)
                    {
                        // OperationStatus.Item2 包含 JSON 格式的数据
                        return Ok();
                    }
                    else
                    {
                        return BadRequest(OperationStatus.Item2);
                    }
                }
                else
                {
                    return BadRequest("用户ID错误");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [Route("CheckSign")]
        [HttpGet]
        public IActionResult CheckSign([FromQuery] int userID, [FromQuery] string itemID)
        {
            try
            {
                ClaimCenter ItemOpObj = new ClaimCenter();
                Tuple<bool, Tuple<bool, string>> operationStatus;
                {
                    operationStatus = ItemOpObj.CheckSignStatus(itemID, userID);
                    ItemOpObj.ReleaseSQLConn();
                }

                if (operationStatus.Item1)
                {
                    // 创建一个包含签署状态的对象
                    var result = new
                    {
                        Item1 = true, // 操作成功
                        Item2 = new
                        {
                            Item1 = operationStatus.Item2.Item1, // 是否已签署
                            Item2 = operationStatus.Item2.Item2  // 错误原因（如果有）
                        }
                    };

                    // 无论是否已签署，都返回 Ok 状态，但包含不同的内容
                    return Ok(result);
                }
                else
                {
                    // 操作失败
                    var result = new
                    {
                        Item1 = false,
                        Item2 = new
                        {
                            Item1 = false,
                            Item2 = operationStatus.Item2.Item2 // 错误原因
                        }
                    };
                    return Ok(result); // 使用 Ok 而不是 BadRequest，让前端处理错误
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                var result = new
                {
                    Item1 = false,
                    Item2 = new
                    {
                        Item1 = false,
                        Item2 = ex.Message
                    }
                };
                return Ok(result); // 使用 Ok 而不是 BadRequest，让前端处理错误
            }
        }

        [Route("DeleteClaim")]
        [HttpDelete]
        public IActionResult DeleteClaim([FromQuery] string itemID)
        {
            try
            {
                ClaimCenter ItemOpObj = new ClaimCenter();
                Tuple<bool, string> operationStatus;
                {
                    operationStatus = ItemOpObj.DeleteItem(itemID);
                    ItemOpObj.ReleaseSQLConn();
                }
                if (operationStatus.Item1)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(operationStatus.Item2);
                
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }


    }
}
