using DatabaseProject.BusinessLogicLayer.CommunityFeatureBLL;
using SQLOperation.PublicAccess.Utilities;
using SQLOperation.PublicAccess.Templates.SQLManager;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using SQLOperation.BusinessLogicLayer.ManagementFeatureBLL;
using System.Transactions;
using System.Data.Common;

namespace DatabaseProject.BusinessLogicLayer.ServiceLayer.ConmmunityFeature
{
    public class UserVIP {
        private CommunityFeatureBusiness<VIP_Members> VIP_MembersBusiness;
        private CommunityFeatureBusiness<VIP_Orders> VIP_OrdersBusiness;
        private Connection VIPConnection;

        private List<string> VIP_MembersList=new List<string> { "user_id","vip_start_date","vip_end_date","status"};
        private List<string> VIP_OrderList = new List<string> {"user_id","total_amount","point_return","order_time","recharge_time" };

        // 构造函数
        public UserVIP(Connection connection)
        {
            VIP_MembersBusiness = new CommunityFeatureBusiness<VIP_Members>(connection);
            VIP_OrdersBusiness = new CommunityFeatureBusiness<VIP_Orders>(connection);
            VIPConnection = connection;
        }

        // 判断用户是否为VIP会员 使用vip_end_date和当前时间进行比较 并且status为active 如果逾期要修改状态为“逾期”
        public int IsVIP(int userId)
        {
            try
            {
                var condition = new Dictionary<string, object> { { "user_id", userId }, { "status", "Active" } };
                var vipMember = VIP_MembersBusiness.QueryBusiness(condition, "AND").FirstOrDefault();
                if (vipMember != null)
                {
                    if(vipMember.VIP_End_Date < DateTime.Now)
                    {
                        // 更新状态为“逾期”
                        var updateParams = new Dictionary<string, object> { { "status", "Inactive" } };
                        VIP_MembersBusiness.UpdateBusiness(updateParams, condition);
                        return -1;
                    }
                    else
                    {
                        return vipMember.VIP_Member_ID;
                    }
                    
                }
                return -1;
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"判断用户是否为VIP会员时发生错误: {ex.Message}");
                throw new ApplicationException("判断用户是否为VIP会员时发生错误，请稍后再试。", ex);
            }
        }

        // 用户充值vip 传入user_id recharge_time total_amount 返回订单的基本信息VIP_Order
        public Tuple<VIP_Orders, DateTime> RechargeVIP(int userId, int rechargeTime, double totalAmount)
        {
            using (var transaction = VIPConnection.GetOracleConnection().BeginTransaction())
            {
                try
                {
                    DateTime startTime = DateTime.Now;
                    // 计算结束日期
                    var endDate = startTime.AddMonths(rechargeTime);

                    // 创建 VIP 订单
                    var vipOrder = VIP_OrdersBusiness.PackageData(0, userId, totalAmount, 0, DateTime.Now, rechargeTime);
                    var orderId = VIP_OrdersBusiness.AddBusiness(VIP_OrderList, "order_id", vipOrder);
                    if (orderId <= 0)
                    {
                        throw new ApplicationException("创建VIP订单时发生错误");
                    }

                    // 查询vip会员信息
                    var condition = new Dictionary<string, object> { { "user_id", userId }, { "status", "Active" } };
                    var vipMember = VIP_MembersBusiness.QueryBusiness(condition, "AND").FirstOrDefault();

                    if (vipMember != null)
                    {
                        if (vipMember.VIP_End_Date >= DateTime.Now)
                        {
                            // 接着当前活跃时间之后
                            startTime = vipMember.VIP_End_Date;
                            endDate = startTime.AddMonths(rechargeTime);
                            // 更新 VIP 会员信息
                            var updateParams = new Dictionary<string, object>
                            {
                                { "vip_end_date", endDate },
                                { "status", "Active" }
                            };
                            var condition_1 = new Dictionary<string, object>
                            {
                                { "vip_member_id", vipMember.VIP_Member_ID }
                            };
                            VIP_MembersBusiness.UpdateBusiness(updateParams, condition_1);

                            // 返回订单的基本信息
                            vipOrder.Order_ID = orderId;

                            transaction.Commit();

                            return new Tuple<VIP_Orders, DateTime>(vipOrder, startTime);
                        }
                       
                    }
                    // 创建新的 VIP 会员
                    var newVipMember = VIP_MembersBusiness.PackageData(0, userId, "Active", startTime, endDate);
                    var vipMemberId = VIP_MembersBusiness.AddBusiness(VIP_MembersList, "vip_member_id", newVipMember);
                    if (vipMemberId <= 0)
                    {
                        throw new ApplicationException("新增VIP会员时发生错误");
                    }

                    // 返回订单的基本信息
                    vipOrder.Order_ID = orderId;

                    // 完成事务
                    transaction.Commit();

                    return new Tuple<VIP_Orders, DateTime>(vipOrder, startTime);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"用户充值VIP时发生错误: {ex.Message}");
                    throw new ApplicationException("用户充值VIP时发生错误，请稍后再试。", ex);
                }
            }
        }


        // 查找user_id对应的用户VIP特征返回list<vip_members> 只查找包含当前时间段的
        public List<VIP_Members> GetVIPInfo(int userId)
        {
            try
            {
                var currentTime = DateTime.Now;
                // 定义where子句
                string whereClause = "USER_ID = :userId AND VIP_START_DATE <= :currentTime AND VIP_END_DATE > :currentTime";
                // 定义参数
                OracleParameter[] parameters = new OracleParameter[]
                {
                    new OracleParameter("userId", userId),
                    new OracleParameter("currentTime", currentTime)
                };

                // 查询数据库获取VIP信息
                var vipInfoList = VIP_MembersBusiness.QueryTableWithWhereBusiness(whereClause, parameters);
                return vipInfoList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"查找用户VIP特征时发生错误: {ex.Message}");
                throw new ApplicationException("查找用户VIP特征时发生错误，请稍后再试。", ex);
            }
        }

        // 新增vip会员 传入user_id vip_start_date vip_end_date vip_status 传出vip_member_id和status
        public int AddVIPMember(int userId, DateTime startDate, DateTime endDate, string status)
        {
            try
            {
                
                // 如果当前没有VIP状态
                if (GetVIPInfo(userId).Count==0)
                {
                    var vipMember = VIP_MembersBusiness.PackageData(0, userId, status, startDate, endDate);
                    var memberId = VIP_MembersBusiness.AddBusiness(VIP_MembersList, "vip_member_id", vipMember);
                    if (memberId <= 0)
                    {
                        throw new ApplicationException("新增VIP会员时发生错误");
                    }
                    return memberId;
                }
                else
                {
                    throw new Exception("新增VIP会员时发生错误，当前用户已经存在VIP");
                }
              
            }
            catch (Exception ex)
            {
                Console.WriteLine($"新增VIP会员时发生错误: {ex.Message}");
                throw new Exception($"新增VIP会员时发生错误: {ex.Message}");
            }
        }


        // 删除vip会员 传入vip_member_id 
        public bool DeleteVIPMember(int vipMemberId)
        {
            try
            {
                var result = VIP_MembersBusiness.RemoveBusiness(new Dictionary<string, object> { { "vip_member_id", vipMemberId } });
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"删除VIP会员时发生错误: {ex.Message}");
                throw new ApplicationException("删除VIP会员时发生错误，请稍后再试。", ex);
            }
        }


        // 修改vip会员 传入需要修改的参数和值对（dictionary)形式
        public bool UpdateVIPMember(int vipMemberId, Dictionary<string, object> updateParams)
        {
            try
            {
                var conditionParams = new Dictionary<string, object> { { "vip_member_id", vipMemberId } };
                var affectedRows = VIP_MembersBusiness.UpdateBusiness(updateParams, conditionParams);
                return affectedRows>0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"修改VIP会员时发生错误: {ex.Message}");
                throw new ApplicationException("修改VIP会员时发生错误，请稍后再试。", ex);
            }
        }


        //管理员查看所有用户的vip信息
        public List<VIP_Members> GetAllVIPMembers()
        {
            try
            {
                // 参数为空数组
                OracleParameter[] parameters = Array.Empty<OracleParameter>();
                return VIP_MembersBusiness.QueryTableWithWhereBusiness("1=1", parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"查看所有VIP会员信息时发生错误: {ex.Message}");
                throw new ApplicationException("查看所有VIP会员信息时发生错误，请稍后再试。", ex);
            }
        }
    }

}
