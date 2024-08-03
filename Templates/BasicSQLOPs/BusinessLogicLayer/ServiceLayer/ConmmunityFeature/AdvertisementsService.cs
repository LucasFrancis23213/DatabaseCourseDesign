using DatabaseProject.BusinessLogicLayer.CommunityFeatureBLL;
using Oracle.ManagedDataAccess.Client;
using SQLOperation.PublicAccess.Templates.SQLManager;
using SQLOperation.PublicAccess.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseProject.BusinessLogicLayer.ServiceLayer.ConmmunityFeature
{
    public class AdvertisementsService {

        private CommunityFeatureBusiness<Advertisements> AdvertisementsBusiness;
        private CommunityFeatureBusiness<Ad_Click_Statistics> Ad_Click_StatisticsBusiness;
        private CommunityFeatureBusiness<Ad_Show_Statistics> Ad_Show_StatisticsBusiness;

        private List<string> AdvertisementsList= new List<string> { "ad_content","ad_picture","ad_url","ad_type","start_time","end_time"};
        private List<string> AdClickStatisticsList = new List<string> { "ad_id","user_id","click_time","ip_address"};
        private List<string> AdShowStatisticsList = new List<string> {"ad_id","user_id","time" };

        // 构造函数
        public AdvertisementsService(Connection connection)
        {
            AdvertisementsBusiness = new CommunityFeatureBusiness<Advertisements>(connection);
            Ad_Click_StatisticsBusiness = new CommunityFeatureBusiness<Ad_Click_Statistics>(connection);
            Ad_Show_StatisticsBusiness = new CommunityFeatureBusiness<Ad_Show_Statistics>(connection);
        }

        // 用户根据click最高呈现一个广告或随机呈现一个广告
        public Advertisements GetAdForUser(int userId)
        {
            try
            {
                // 随机选择showAd
                var random = new Random();
                var showAd = random.Next(0, 2) == 0;
                Advertisements advertisement=null;
                DateTime currentTime= DateTime.Now;

                if (showAd)
                {
                    // 查询广告和展示统计表，返回点击次数最多的广告
                    var selectClause = @"
                        a.AD_ID AS AD_ID,
                        a.AD_CONTENT AS AD_CONTENT,
                        a.AD_PICTURE AS AD_PICTURE,
                        a.AD_URL AS AD_URL,
                        a.AD_TYPE AS AD_TYPE,
                        a.START_TIME AS START_TIME,
                        a.END_TIME AS END_TIME,
                        COALESCE(COUNT(s.AD_ID), 0) AS CLICK_COUNT";
                    var fromClause = "ADVERTISEMENTS a LEFT OUTER JOIN AD_CLICK_STATISTICS s ON a.AD_ID = s.AD_ID";
                    var whereClause = @"
                        a.END_TIME >= :currentTime AND a.START_TIME < :currentTime
                        GROUP BY a.AD_ID, a.AD_CONTENT, a.AD_PICTURE, a.AD_URL, a.AD_TYPE, a.START_TIME, a.END_TIME
                        ORDER BY CLICK_COUNT DESC";
                    // 创建参数
                    var parameters = new[] { new OracleParameter("currentTime", currentTime) };
                    var result = AdvertisementsBusiness.QueryTableWithSelectBusiness(selectClause, fromClause, whereClause, parameters);
                    

                    // 获取展示次数最多的广告
                    var topAd = result.FirstOrDefault();
                    if (topAd != null)
                    {
                        advertisement=AdvertisementsBusiness.MapDictionaryToObject(topAd);
                    }

                }
                else
                {
                    // 随机选择一个广告
                    var whereClause = "END_TIME >= :currentDate AND START_TIME < :currentDate";
                    var parameters = new[] { new OracleParameter("currentDate", currentTime) };
                    var allAds = AdvertisementsBusiness.QueryTableWithWhereBusiness(whereClause,parameters);
                    if (allAds.Count() > 0)
                    {
                        advertisement=allAds[random.Next(allAds.Count)];
                    }
                }

                // 有一个广告展示
                if(advertisement == null)
                {
                    throw new Exception("当前没有广告");
                }
                else
                {
                    // 更新广告的展示统计
                    var adId = advertisement.Ad_ID;
                    var currentDate = DateTime.Now;

                    var newStats = Ad_Show_StatisticsBusiness.PackageData(adId,userId,  currentDate);
                    Ad_Show_StatisticsBusiness.AddBusiness(AdShowStatisticsList, "ad_id", newStats);

                    return advertisement;
                }

                
            }
            catch(Exception ex)
            {
                Console.WriteLine($"获取广告时发生错误: {ex.Message}");
                throw new Exception($"获取广告时发生错误: {ex.Message}");
            }
           
   
        }

        // 用户点击广告
        public bool UserClickAd(int userId, int adId, DateTime clickTime, string ipAddress)
        {
            try
            {
                var click = Ad_Click_StatisticsBusiness.PackageData(adId,userId, clickTime, ipAddress);

                var result=Ad_Click_StatisticsBusiness.AddBusiness(AdClickStatisticsList, "ad_id", click);

                // 返回0说明记录失误
                return result > 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"记录广告点击时发生错误: {ex.Message}");
                throw new ApplicationException("记录广告点击时发生错误", ex);
            }
            
        }

        // 管理员增加广告 传入ad_content, ad_picture, ad_url, ad_type, start_time, end_time
        public int AddAdvertisement(string content, string picture, string url, string type, DateTime startTime, DateTime endTime)
        {
            try
            {
                var ad = AdvertisementsBusiness.PackageData(0,content,picture,url,type,startTime,endTime);

                var result = AdvertisementsBusiness.AddBusiness(AdvertisementsList, "ad_id", ad);

                if (result > 0)
                {
                    return result;
                }
                else
                {
                    throw new ApplicationException("添加广告时发生错误，请稍后再试。");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"添加广告时发生错误: {ex.Message}");
                throw new Exception(ex.Message);
            }
        }


        // 管理员对广告进行更新 传入ad_id 需要更新的字段名 以及需要更新的值（后者可能为多个）
        public bool UpdateAdvertisement(int adId, Dictionary<string, object> updateFields)
        {
            try
            {
                var condition = new Dictionary<string, object> { { "ad_id", adId } };
                var result = AdvertisementsBusiness.UpdateBusiness(updateFields, condition);
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新广告时发生错误: {ex.Message}");
                throw new ApplicationException("更新广告时发生错误，请稍后再试。", ex);
            }
        }

        // 管理员删除广告 传入ad_id 进行删除 
        public bool RemoveAdvertisement(int adId)
        {
            try
            {
                var condition = new Dictionary<string, object> { { "ad_id", adId } };
                return AdvertisementsBusiness.RemoveBusiness(condition);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"删除广告时发生错误: {ex.Message}");
                throw new ApplicationException(ex.Message);
            }
        }



        // 管理员查看广告相关信息 不传入参数 返回list<advertisements>
        public List<Advertisements> GetAdvertisementInfo()
        {
            try
            {
                var whereClause = "1=1"; // 查询所有广告
                var adsData = AdvertisementsBusiness.QueryTableWithWhereBusiness(whereClause, null);
                return adsData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取广告信息时发生错误: {ex.Message}");
                throw new ApplicationException("获取广告信息时发生错误，请稍后再试。", ex);
            }
        }




        // 管理员查看广告的详细信息 连表advertisements ad_click_statistics和ad_show_statistics 都使用外连接
        //然后使用聚集函数找出click_count和show_count
        public AdvertisementsDetails GetAdDetails(int adId)
        {
            try
            {

                var selectClause = @"
                        A.AD_ID AS AD_ID,
                        A.AD_CONTENT AS AD_CONTENT,
                        A.AD_PICTURE AS AD_PICTURE,
                        A.AD_URL AS AD_URL,
                        A.AD_TYPE AS AD_TYPE,
                        A.START_TIME AS START_TIME,
                        A.END_TIME AS END_TIME,
                        COALESCE(COUNT(DISTINCT CS.USER_ID || CS.CLICK_TIME), 0) AS CLICK_COUNT,
                        COALESCE(COUNT(DISTINCT SS.USER_ID || SS.TIME), 0) AS SHOW_COUNT";
                var fromClause = @"
                    ADVERTISEMENTS A 
                    LEFT OUTER JOIN AD_CLICK_STATISTICS CS ON A.AD_ID = CS.AD_ID 
                    LEFT OUTER JOIN AD_SHOW_STATISTICS SS ON A.AD_ID = SS.AD_ID";
                var whereClause = @"
                    A.AD_ID = :adId 
                    GROUP BY 
                        A.AD_ID,
                        A.AD_CONTENT,
                        A.AD_PICTURE,
                        A.AD_URL,
                        A.AD_TYPE,
                        A.START_TIME,
                        A.END_TIME";
                var parameters = new[] { new OracleParameter("adId", adId) };
                var result = AdvertisementsBusiness.QueryTableWithSelectBusiness(selectClause, fromClause, whereClause, parameters);
                var adDetails = result.FirstOrDefault();

                if (adDetails != null)
                {
                    var advertisementDetails = new AdvertisementsDetails
                    {
                        Advertisements=AdvertisementsBusiness.MapDictionaryToObject(adDetails),
                        Click_Count = int.Parse(adDetails["CLICK_COUNT"].ToString()),
                        Show_Count = int.Parse(adDetails["SHOW_COUNT"].ToString())
                    };

                    return advertisementDetails;
                }
                else
                {
                    throw new ApplicationException("未找到广告详情。");
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取广告详细信息时发生错误: {ex.Message}");
                throw new ApplicationException(ex.Message);
            }
        }

        // 管理员查看广告的详细点击信息 传入adId
        public List<Ad_Click_Statistics> GetAdClickDetails(int adId)
        {
            try
            {
                var condition = new Dictionary<string, object> { { "ad_id", adId } };
                var clickDetails = Ad_Click_StatisticsBusiness.QueryBusiness(condition, "1=1");

                return clickDetails;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取广告点击详细信息时发生错误: {ex.Message}");
                throw new ApplicationException("获取广告点击详细信息时发生错误，请稍后再试。", ex);
            }
        }
    }

}
