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

        private List<string> AdvertisementsList= new List<string> { "ad_content","ad_picture","ad_url","ad_type","start_time","end_time","click_count","show_count"};
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
                Advertisements? advertisement=null;
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
                        a.CLICK_COUNT AS CLICK_COUNT,
                        a.SHOW_COUNT AS SHOW_COUNT,
                        CASE WHEN a.SHOW_COUNT > 0 THEN a.CLICK_COUNT / a.SHOW_COUNT ELSE 0 END AS CLICK_RATE";  // 计算点击率

                    var fromClause = "ADVERTISEMENTS a";

                    var whereClause = @"
                        a.END_TIME >= :currentTime AND a.START_TIME < :currentTime
                        ORDER BY CLICK_RATE DESC";  // 根据点击率排序
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

                    // 0占位
                    var newStats = Ad_Show_StatisticsBusiness.PackageData(0,adId,userId,  currentDate);
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
                var click = Ad_Click_StatisticsBusiness.PackageData(0,adId,userId, clickTime, ipAddress);

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
                var ad = AdvertisementsBusiness.PackageData(0,content,picture,url,type,startTime,endTime,0,0);

                var result = AdvertisementsBusiness.AddBusiness(AdvertisementsList, "ad_id", ad);

                if (result > 0)
                {
                    return result;
                }
                else
                {
                    throw new Exception("数据库添加错误");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"添加广告时发生错误: {ex.Message}");
                throw new Exception($"添加广告时发生错误: {ex.Message}");
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
                throw new Exception($"删除广告时发生错误: {ex.Message}");
            }
        }



        // 管理员查看广告相关信息 不传入参数 返回list<advertisements>
        public List<Advertisements> GetAdvertisementInfo()
        {
            try
            {
                var whereClause = "1=1"; // 查询所有广告
                // 参数为空
                OracleParameter[] parameters = Array.Empty<OracleParameter>();
                var adsData = AdvertisementsBusiness.QueryTableWithWhereBusiness(whereClause, parameters);
                return adsData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取广告信息时发生错误: {ex.Message}");
                throw new Exception($"获取广告信息时发生错误: {ex.Message}");
                
            }
        }



        // 管理员查看广告的详细信息 连表advertisements ad_click_statistics和ad_show_statistics 都使用外连接
        //然后使用聚集函数找出click_count和show_count
        public Advertisements GetAdDetails(int adId)
        {
            try
            {
                // 构建查询条件
                var condition = new Dictionary<string, object>
                    {
                        { "AD_ID", adId }
                    };

                // 使用 QueryBusiness 方法查询广告详情
                var result = AdvertisementsBusiness.QueryBusiness(condition, "AND");
                var adDetails = result.FirstOrDefault();

                if (adDetails != null)
                {
                    return adDetails;
                }
                else
                {
                    throw new Exception("未找到广告详情。");
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取广告详细信息时发生错误: {ex.Message}");
                throw new Exception($"获取广告详细信息时发生错误: {ex.Message}");
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
                throw new Exception($"获取广告点击详细信息时发生错误: {ex.Message}");
            }
        }
    }

}
