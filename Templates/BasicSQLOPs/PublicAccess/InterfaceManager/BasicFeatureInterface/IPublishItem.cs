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
        //lostItems:丢失物品基础信息，对应表Lost_Item
        //itemImages:丢失物品图片信息，和lostItems一一对应，对应表Item_Images
        // rewardOffers:悬赏选项选择信息，对应表Reward_offer
        //reward_or_not:是否选择了添加悬赏
        public Tuple<bool, string> PublishLostItem(List<Lost_Item> lostItems, List<Reward_Offers> rewardOffers, bool reward_or_not);
        //发布失物招领
        //foundItems:无主物品基础信息，对应表Found_Item
        //itemImages:无主物品图片信息，和foundItems一一对应，对应表Item_Images
        public Tuple<bool, string> PublishFoundItem(List<Found_Item> foundItems);
    }
}
