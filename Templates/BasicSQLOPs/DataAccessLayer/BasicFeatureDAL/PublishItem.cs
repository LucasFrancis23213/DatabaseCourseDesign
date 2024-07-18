using SQLOperation.PublicAccess.Templates.SQLManager;
using SQLOperation.PublicAccess.Utilities;
using SQLOperation.PublicAccess.Templates.TemplateInterfaceManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLOperation.BusinessLogicLayer.BasicFeatureBLL
{

    public class PublishItem : IPublishItem
    {
        //发布寻物启事基础表单
        //假设已审核通过是1
        private bool PublishLostItemBasic(Lost_Item item)
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
            //未审核
            if (item.Review_Status == 0)
            {
                Debug.WriteLine("物品基础信息尚未被审核通过！");
                return false;
            }
            //已审核的就插入第一个表单
            else
            {
                if (basic.InsertOperation("Lost_Item", lostNames, values))
                    return true;
                //插入失败
                else
                {
                    Debug.WriteLine("插入失败");
                    return false;
                }
            }

        }
        //插入物品图片
        private bool InsertImage(Item_Images item)
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
            //未审核
            if (item.Review_Status == 0)
            {
                Debug.WriteLine("图片尚未被审核通过！");
                return false;
            }
            //已审核的就插入第一个表单
            else
            {
                if (basic.InsertOperation("Item_Images", Names, values))
                    return true;
                //插入失败
                else
                {
                    Debug.WriteLine("插入失败");
                    return false;
                }
            }
        }
        //选择设置悬赏找物品
        private bool HaveReward(Reward_Offers item)
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
            if (basic.InsertOperation("Reward_Offers", Names, values))
                return true;
            //插入失败
            else
            {
                Debug.WriteLine("插入失败");
                return false;
            }

        }
        //失物招领基础信息表填写
        private bool PublistFoundItemBasic(Found_Item item)
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
            //未审核
            if (item.Review_Status == 0)
            {
                Debug.WriteLine("物品基础信息尚未被审核通过！");
                return false;
            }
            //已审核的就插入第一个表单
            else
            {
                if (basic.InsertOperation("Lost_Item", Names, values))
                    return true;
                //插入失败
                else
                {
                    Debug.WriteLine("插入失败");
                    return false;
                }
            }
        }

        //外部接口函数
        public bool PublishLostItem(List<Lost_Item> item1, List<Item_Images> item2, List<Reward_Offers> item3, bool reward_or_not)
        {
            int n = 0;
            foreach (Lost_Item item in item1) {
                
                //先插入基础表单
                if (PublishLostItemBasic(item))
                {
                    //基础表单插入成功，插入对应图片
                    if (InsertImage(item2[n]))
                    {
                        //图片插入成功，插入是否悬赏
                        if(!reward_or_not)
                        {
                            continue;
                        }
                        //有悬赏
                        else
                        {
                            if (HaveReward(item3[n]))
                            {
                                n++;
                                continue;
                            }
                            else
                            {
                                Debug.WriteLine("悬赏设置失败！");
                                return false;
                            }
                        }
                    }
                    else
                    { return false; }
                    
                }
                else
                { return false; }
            }
            return true ;
        }

        public bool PublishFoundItem(List<Found_Item> item1, List<Item_Images> item2)
        {
            int n = 0;
            foreach (Found_Item item in item1)
            {
                //先插入基础表单
                if (PublistFoundItemBasic(item))
                {
                    //基础表单插入成功，插入对应图片
                    if (InsertImage(item2[n]))
                    {
                        n++;
                        continue;
                    }
                    else
                    { return false;}
                }
                else
                { return false; }
            }
            return true;
        }
    }


}
