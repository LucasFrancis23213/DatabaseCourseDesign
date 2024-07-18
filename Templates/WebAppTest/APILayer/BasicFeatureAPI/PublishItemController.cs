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
    public class PublishItemController
    {
        //private Connection conn;
        //private OracleConnection OracleConnection;
        private PublishItem PublishItemObject;


        [Route("api/PublishItem/POST")]
        [HttpPost]
        public bool PublishItemOPs([FromBody] dynamic InputPubJson)
        {
            try
            {
                string PubString = InputPubJson.ToString();
                JObject TmpJson = JObject.Parse(PubString);

                try
                {
                    PublishItemObject = TmpJson.ToObject<PublishItem>();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Json Deserialization Error: {ex.Message}");
                    return false;
                }
                //Tuple<bool, string> OperationStatus = PublishItemObject.PublishFoundItem();
                //return OperationStatus.Item1;
                Debug.WriteLine($"Test Completeeeeeeeeee");
                return true;
               
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"In PublishItem Function,报错为：{ex.Message}");
                return false;
            }
        }
    }
}
