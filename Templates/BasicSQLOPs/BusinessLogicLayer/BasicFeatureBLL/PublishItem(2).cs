using SQLOperation.PublicAccess.Templates.SQLManager;
using SQLOperation.PublicAccess.Utilities;
using SQLOperation.PublicAccess.Templates.TemplateInterfaceManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

//查找是否重名，ID自增自减？
namespace SQLOperation.BusinessLogicLayer.BasicFeatureBLL
{

    public class PublishItem : IPublishItem
    {
        //发布寻物启事基础表单
        private Tuple<bool, string> PublishLostItemBasic(Lost_Item item)
        {
            var lostNames = new List<string>
            {
            "Item_ID",
            "Item_Name",
            "Category_ID",
            "Description",
            "Lost_Location",
            "Lost_Date",
            "User_ID",
            "Lost_Status",
            "Review_Status"
            };
            var values = new List<object>
            {
                item.Item_ID,
                item.Item_Name,
                item.Category_ID,
                item.Description,
                item.Lost_Location,
                item.Lost_Date,
                item.User_ID,
                item.Lost_Status,
                item.Review_Status
            };
            BasicSQLOps basic = new BasicSQLOps();
            var result = basic.InsertOperation("Lost_Item", lostNames, values);
            bool isSuccess = result.Item1; // 获取是否成功插入
            string errorReason = result.Item2; // 获取出错误原因
            if (isSuccess)
                return new Tuple<bool, string>(true, string.Empty);
            //插入失败
            else
            {
                return new Tuple<bool, string>(false, errorReason);
            }
        }
        //插入物品图片
        private Tuple<bool, string> InsertImage(Item_Images item)
        {
            var Names = new List<string>
            {
            "Image_ID",
            "Image_URL",
            "Item_ID",
            "Description",
            "Review_Status"
            };
            var values = new List<object>
            {
                item.Image_ID,
                item.Image_URL,
                item.Item_ID,
                item.Description,
                item.Review_Status
            };
            BasicSQLOps basic = new BasicSQLOps();
            var result = basic.InsertOperation("Item_Images", Names, values);
            bool isSuccess = result.Item1; // 获取是否成功插入
            string errorReason = result.Item2; // 获取出错误原因
            if (isSuccess)
                return new Tuple<bool, string>(true, string.Empty);
            //插入失败
            else
            {
                return new Tuple<bool, string>(false, errorReason);
            }
        }
        //选择设置悬赏找物品
        private Tuple<bool, string> HaveReward(Reward_Offers item)
        {
            var Names = new List<string>
            {
            "User_ID",
            "Item_ID",
            "Reward_Amount",
            "Status",
            "Release_Date",
             "Deadline"
            };
            var values = new List<object>
            {
                item.User_ID,
                item.Item_ID,
                item.Reward_Amount,
                item.Status,
                item.Release_Date,
                item.Deadline,
            };
            BasicSQLOps basic = new BasicSQLOps();
            var result = basic.InsertOperation("Reward_Offers", Names, values);
            bool isSuccess = result.Item1; // 获取是否成功插入
            string errorReason = result.Item2; // 获取出错误原因
            if (isSuccess)
                return new Tuple<bool, string>(true, string.Empty);
            //插入失败
            else
            {
                return new Tuple<bool, string>(false, errorReason);
            }

        }
        //失物招领基础信息表填写
        private Tuple<bool, string> PublistFoundItemBasic(Found_Item item)
        {
            var Names = new List<string>
            {
                "Item_ID",
                "Item_Name",
                "Category_ID",
                "Description",
                "Found_Location",
                "Found_Date",
                "User_ID",
                "Match_Status",
               " Review_Status",
            };
            var values = new List<object>
            {
                item.Item_ID,
                item.Item_Name,
                item.Category_ID,
                item.Description,
                item.Found_Location,
                item.Found_Date,
                item.User_ID,
                item.Match_Status,
                item.Review_Status
            };
            BasicSQLOps basic = new BasicSQLOps();
            var result = basic.InsertOperation("Found_Item", Names, values);
            bool isSuccess = result.Item1; // 获取是否成功插入
            string errorReason = result.Item2; // 获取出错误原因
            if (isSuccess)
                return new Tuple<bool, string>(true, string.Empty);
            //插入失败
            else
            {
                return new Tuple<bool, string>(false, errorReason);
            }

        }

        //外部接口函数
        public Tuple<bool, string> PublishLostItem(List<Lost_Item> lostItems, List<Item_Images> itemImages, List<Reward_Offers> rewardOffers, bool reward_or_not)
        {
            int n = 0;
            try{
                foreach (Lost_Item item in lostItems) {
                //先插入基础表单
                var basicExcel = PublishLostItemBasic(item);
                bool isSuccess1 = basicExcel.Item1; // 获取是否成功插入
                string errorReason1 = "表单写入数据库过程中" + basicExcel.Item2; // 获取出错误原因
                if (isSuccess1)
                {
                    //基础表单插入成功，插入对应图片
                    var insertImage = InsertImage(itemImages[n]);
                    bool isSuccess2 = insertImage.Item1; // 获取是否成功插入
                    string errorReason2 = "图片插入数据库过程中"+insertImage.Item2; // 获取出错误原因
                    if (isSuccess2)
                    {
                        //图片插入成功，插入是否悬赏
                        if(!reward_or_not)
                        {
                          n++;
                          continue; 
                        }
                        //有悬赏
                        else
                        {
                            var reward = HaveReward(rewardOffers[n]);
                            bool isSuccess3 = reward.Item1; // 获取是否成功插入
                            string errorReason3 = "悬赏设置过程中"+reward.Item2; // 获取出错误原因
                            if (isSuccess3)
                            {
                                n++;
                                continue;
                            }
                            //悬赏设置失败
                            else
                            { return new Tuple<bool, string>(false, errorReason3); }
                        }
                    }
                    //插入图片失败
                    else
                    { return new Tuple<bool, string>(false, errorReason2); }
                    
                }
                //插入基础表单失败
                else
                { return new Tuple<bool, string>(false, errorReason1); }
            }
                return new Tuple<bool, string>(true, string.Empty); 
            }
            catch (Exception ex)
            {
                //其他未处理的异常
                return new Tuple<bool, string>(false, "发布寻物启事发生异常: " + ex.Message);
            }
        }

        public Tuple<bool, string> PublishFoundItem(List<Found_Item> foundItems, List<Item_Images> itemimages)
        {
            int n = 0;
            try
            {foreach (Found_Item item in foundItems)
            {
                //先插入基础表单
                var basicExcel = PublistFoundItemBasic(item);
                bool isSuccess1 = basicExcel.Item1; // 获取是否成功插入
                string errorReason1 = "表单写入数据库过程中" + basicExcel.Item2; // 获取出错误原因
                if (isSuccess1)
                {
                    //基础表单插入成功，插入对应图片
                    var insertImage = InsertImage(itemimages[n]);
                    bool isSuccess2 = insertImage.Item1; // 获取是否成功插入
                    string errorReason2 = "图片插入数据库过程中" + insertImage.Item2; // 获取出错误原因
                    if (isSuccess2)
                    {
                        n++;
                        continue;
                    }
                    //插入图片失败
                    else
                    { return new Tuple<bool, string>(false, errorReason2); }
                }
                //插入基础表单失败
                else
                { return new Tuple<bool, string>(false, errorReason1); }
            }
                return new Tuple<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {
                //其他未处理的异常
                return new Tuple<bool, string>(false, "发布失物招领发生异常: " + ex.Message);
            }
        }
    }
    

}
