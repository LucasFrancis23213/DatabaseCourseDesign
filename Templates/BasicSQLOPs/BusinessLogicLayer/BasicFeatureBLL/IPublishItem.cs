using SQLOperation.PublicAccess.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLOperation.BusinessLogicLayer.BasicFeatureBLL
{
    public interface IPublishItem {
        //发布寻物启事
        //item1:丢失物品基础信息，对应表Lost_Item
        //item2:丢失物品图片信息，和item1一一对应，对应表Item_Images
        //item3:悬赏选项选择信息，对应表Reward_offer
        //reward_or_not:是否选择了添加悬赏
        //rewardLostQueue:审核队列，实质上是一个数组,前面三个参数都在list的一项里面，是用逗号隔开的string
        public Tuple<bool, string> PublishLostItem(List<Lost_Item> item1, List<Item_Images> item2, List<Reward_Offers> item3, bool reward_or_not,List<object>reviewLostQueue);
        //发布失物招领
        //item1:无主物品基础信息，对应表Found_Item
        //item2:无主物品图片信息，和item1一一对应，对应表Item_Images
        //rewardFoundQueue:审核队列，实质上是一个数组,前面2个参数都在list的一项里面，是用逗号隔开的string
        public Tuple<bool, string> PublishFoundItem(List<Found_Item> item1, List<Item_Images> item2,List<object>reviewFoundQueue);
    }
}
