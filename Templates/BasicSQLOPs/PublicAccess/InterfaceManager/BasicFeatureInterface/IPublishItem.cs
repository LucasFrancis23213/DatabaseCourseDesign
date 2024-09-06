using SQLOperation.PublicAccess.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLOperation.BusinessLogicLayer.BasicFeatureBLL
{
    //对Lost_Item和Found_Item的增删查没有更新操作
    public interface IPublishItem {
        //发布寻物启事
        //lostItems:丢失物品基础信息，对应表Lost_Item
        //itemImages:丢失物品图片信息，和lostItems一一对应，对应表Item_Images
        //rewardOffers:悬赏选项选择信息，对应表Reward_offer
        //reward_or_not:是否选择了添加悬赏
        public Tuple<bool, string> PublishLostItem(List<Lost_Item> lostItems, List<Reward_Offers> rewardOffers);
        //发布失物招领
        //foundItems:无主物品基础信息，对应表Found_Item
        //itemImages:无主物品图片信息，和foundItems一一对应，对应表Item_Images
        public Tuple<bool, string> PublishFoundItem(List<Found_Item> foundItems);
        //用于删除被选择的单元
        //type==0删除表格lost_Item
        //type==1删除表格Found_Item
        //index是一一对应的属性名和值
        //正常来说，如果点击按钮直接删除，就直接index只有itemID应该就可以了
        public Tuple<bool, string> DeleteItem(int type,Dictionary<string,object>index);
        //用于查询物品
        //type同上
        //index是条件，返回默认是*
        public Tuple<bool, string> QueryItem(int type, Dictionary<string, object> index);
        //用于物品审核通过之后的更新操作
        //type同上，itemID是物品ID用于确定是那一条通过了
        public Tuple<bool, string> ReviewItem(int type,List<string> itemID);

       
    }
}
