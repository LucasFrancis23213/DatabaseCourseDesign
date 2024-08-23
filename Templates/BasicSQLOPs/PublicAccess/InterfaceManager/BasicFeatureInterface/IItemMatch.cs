using Oracle.ManagedDataAccess.Client;
using SQLOperation.PublicAccess.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLOperation.PublicAccess.InterfaceManager.BasicFeatureInterface
{
    public interface IItemMatch
    {
        //点击这是我的按钮后直接运行
        public Tuple<bool, string> ItemClaimProcessBasic(Item_Claim_Processes item);
        //进行物品的自动匹配，点击按钮后，可以进行选择，如果我也发布了寻物启事
        public Tuple<bool, string> AutoMatch(List<string> Status, List<Match_Records> MatchRecord);
        //协议阅读表插入，这个不知道应该放在哪个里面怎么对应，直接给出
        public Tuple<bool, string> ItemReturnAgreementBasic(Item_Return_Agreements item);
        //当进行审核的时候，一般都会有这个状态变更，需要调用一下
        //historyid不知道编写方式，所以当作参数传参,单独列出，不用updateitem
        public Tuple<bool, string> UpdateHistory(OracleConnection conn, int itemId, int historyId, string newStatus);
        //物品归还流程
        //status是阅读完协议是否同意的状态，但是我不太懂这个id怎么对应的，所以这里直接给list，麻烦调用时候自行给出状态
        public Tuple<bool, string> ExchangeItem(List<string> Status, List<Item_Exchanges> exchangeItems);
        //用于删除物品声明和物品交换，其他表设置为不可删除模式
        //index是一一对应的属性名和值
        public Tuple<bool, string> DeleteItem(string TableName,Dictionary<string, object> index);
        //用于查询记录
        //TableName=="Item_Status_History"/"Item_Claim_Processes"/"Match_Records"
        //index是条件，返回默认是*
        public Tuple<bool, string> QueryItem(string TableName, Dictionary<string, object> index);
        //更改记录(主要是提供给修改阅读状态和交换状态和归还状态)
        //当物品选择进入旧物市场之后，修改found_item的match_status==“出售”，即会出现在出售物品池。
        public Tuple<bool, string> UpdateItem(string TableName, Dictionary<string, object> UpdateColumns, Dictionary<string, object> index);
    }
   
}
