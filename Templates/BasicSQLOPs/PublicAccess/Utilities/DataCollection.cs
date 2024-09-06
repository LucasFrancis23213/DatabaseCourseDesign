using System.Text.Json.Serialization;

namespace SQLOperation.PublicAccess.Utilities
{
    public class User_Conversations
    {
        public int User1_ID { get; set; }
        public int User2_ID { get; set; }
        public int Last_Message_ID { get; set; }
       

        public int Unread_Count {  get; set; }
    }


    public class User_Messages
    {
        public int Message_ID { get; set; }
        public string Message_Content { get; set; } // Corresponds to CLOB in SQL
        public string Read_Status { get; set; }
        public DateTime Send_Time { get; set; }
        public string Message_Type { get; set; }
        public int Sender_User_ID { get; set; }
        public int Receiver_User_ID { get; set; }
    }

    public class User_Posts
    {
        public int Post_ID { get; set; }
        public int User_ID { get; set; }
        public string Content { get; set; }
        public DateTime Post_Date { get; set; }
        public int Popularity { get; set; }
    }

    public class User_Relationships
    {
        public int Relation_ID { get; set; }
        public int User_ID1 { get; set; }
        public int User_ID2 { get; set; }
        public string Relation_Type { get; set; }
        public DateTime Established_Date { get; set; }
    }

    public class Follow_List
    {
        public int Follow_ID { get; set; }
        public int Follower_User_ID { get; set; }
        public int Followed_User_ID { get; set; }
        public DateTime Follow_Date { get; set; }
    }

    public class Questions
    {
        public int Question_ID { get; set; }
        public string Item_ID { get; set; }
        public int User_ID { get; set; }
        public string Question_Content { get; set; }
        public DateTime Question_Time { get; set; }

       
    }

    public class Answers
    {
        public int Answer_ID { get; set; }
        public int Question_ID { get; set; }
        public int User_ID { get; set; }
        public string Answer_Content { get; set; }
        public DateTime Answer_Date { get; set; }
    }
    public class UserInfo
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class Item_Comments
    {
        public int Comment_ID { get; set; }
        public string Item_ID { get; set; }

        public int User_ID { get; set; }
        public string Content { get; set; }
        public DateTime Datetime { get; set; }
    }

    public class User_Activity {
        public int Activity_ID { get; set; }
        public int User_ID { get; set; }
        public string Activity_Type {  get; set; }

        public int Score {  get; set; }

        public DateTime DateTime { get; set; }
    }

    public class User_Points
    {
        public int User_ID { get; set; }
        public int Points { get; set; }
    }


    public class Lost_Item
    {
        public string Item_ID;
        public string Item_Name;
        public int Category_ID;
        public string Description;
        public string Lost_Location;
        public DateTime Lost_Date;
        public int User_ID;
        public string Lost_Status;
        public int Review_Status;
        public string Image_URL;
        public int Tag_ID;
        public int Is_Rewarded;
    }

    public class Found_Item
    {
        public string Item_ID;
        public string Item_Name;
        public int Category_ID;
        public string Description;
        public string Found_Location;
        public DateTime Found_Date;
        public int User_ID;
        public string Match_Status;
        public int Review_Status;
        public string Image_URL;
        public int Tag_ID;
    }

    public class Reward_Offers
    {
        public int User_ID;
        public string Item_ID;
        public int Reward_Amount;
        public string Status;
        public DateTime Release_Date;// 原来是int
        public DateTime Deadline;// 原来是int
    }
    public class Item_Status_History
    {
        public string History_ID;
        public DateTime Change_Date;
        public string Item_ID;
        public string Preview_Status;
        public string New_Status;
    }

    public class Item_Categories
    {
        public int Category_ID;
        public string Category_Name;
    }

    public class Item_Tags
    {
        public int Tag_ID;
        public string Tag_Name;
    }

    public class Item_Tag_Links
    {
        public int Item_ID;
        public int Tag_ID;
        public int Review_Status;
    }

    public class Item_Images
    {
        public int Image_ID;
        public string Image_URL;
        public int Item_ID;
        public string Description;
        public int Review_Status;
    }

    public class Item_Claim_Processes
    {
        public string Process_ID;
        public string Item_ID;
        public int Claimant_User_ID;
        public string Status;
        public DateTime Application_Date;// 原来是int
        public int Publish_User_ID;
    }

    public class Match_Records
    {
        public int Record_ID;
        public string Lost_Item_ID;
        public string Found_Item_ID;
        public DateTime Match_Date;
        public string Processing_Status;
    }

    public class Item_Exchanges
    {
        public int Exchange_ID;
        public string Lost_Item_ID;
        public string Found_Item_ID;
        public int Initiator_User_ID;
        public string Transaction_Type;
        public int Responder_User_ID;
        public string Exchange_Status;
        public DateTime Creation_Time;// 原来是int
    }

    public class Item_Return_Agreements
    {
        public int Agreement_ID;
        public string Item_ID;
        public int From_User_ID;
        public int To_User_ID;
        public string Agreement_Content;
        public string Exchange_Status;
        public DateTime Creation_Time;//原来是int
    }

    public class Item_Review_Links
    {
        public int Time_Stamp;
        public int User_ID;
        public int Review_ID;
        public int Image_ID;
        public int Tag_ID;
    }

    public class Review_Codes
    {
        public int Review_ID;
        public string Review_Info;
    }

    public class VIP_Members
    {
        public int VIP_Member_ID { get; set; }
        public int User_ID { get; set; }
        public string Status { get; set; }
        public DateTime VIP_Start_Date { get; set; }
        public DateTime VIP_End_Date { get; set; }
    }

    public class VIP_Orders
    {
        public int Order_ID { get; set; }
        public int User_ID { get; set; }
        public double Total_Amount { get; set; }
        public int Point_Return { get; set; }
        public DateTime Order_Time { get; set; }
        public int Recharge_Time { get; set; }
    }

    public class Admin_Edit_VIP
    {
        public int Admin_ID;
        public int VIP_Member_ID;
        public DateTime Edit_Time_Stamp;
        public string Edit_Type;
        public string Edit_Content;//SQL里的数据类型为Text
    }

    public class Advertisements
    {
        public int Ad_ID { get; set; }
        public string Ad_Content { get; set; }
        public string Ad_Picture { get; set; }
        public string Ad_URL { get; set; }
        public string Ad_Type { get; set; }
        public DateTime Start_Time { get; set; }
        public DateTime End_Time { get; set; }
        public int Click_Count { get; set; }

        public int Show_Count { get; set; }
    }

    public class Ad_Click_Statistics
    {
        public int Click_ID { get; set; }
        public int Ad_ID { get; set; }
        public int User_ID { get; set; }
        public DateTime Click_Time { get; set; }
        public string IP_Address { get; set; }
    }

    public class Ad_Show_Statistics
    {
        public int Show_ID { get; set; }
        public int Ad_ID { get; set; }
        public int User_ID { get; set; }
        public DateTime Time { get; set; }
    }

    public class Admin_Edit_Ad
    {
        public int Admin_ID;
        public int Ad_ID;
        public DateTime Edit_Time_Stamp;
        public string Edit_Type;
        public string Edit_Content;
    }

    public class User_Preferences
    {
        public int Preference_ID;
        public int User_ID;
        public string Preference_Type;
        public string Preference_Value;
        public DateTime Release_Date;//原来是int
    }

    public class Users
    {
        [JsonPropertyName("USER_ID")]
        public int User_ID { get; set; }

        [JsonPropertyName("USER_NAME")]
        public string User_Name { get; set; }

        [JsonPropertyName("PASSWORD_")]
        public string Password { get; set; }

        [JsonPropertyName("CONTACT")]
        public string Contact { get; set; }

        [JsonPropertyName("IS_DELETED")]
        public int Is_Deleted { get; set; }

        [JsonPropertyName("AVATAR")]
        public string Avatar { get; set; }
    }

    public class User_Subscriptions
    {
        public int Subscription_ID;
        public int User_ID;
        public string Subsciption_Type;
        public string Subsciption_Status;
        public DateTime Release_Date;// 原来是int
    }


    public class Auth_Info
    {
        public int User_ID { get; set; }
        public DateTime Auth_Date { get; set; }
    }

    public class System_Logs
    {
        public int System_Log_ID;
        public string Operation_Type;
        public string Operation_Details;
        public int User_ID;
    }

    public class API_Access_Logs
    {
        public int Access_ID;
        public string API_Name;
        public string Accessor_ID;
        public DateTime Access_Time;
        public string Access_Result;
    }

    public class Security_Events
    {
        public int Event_ID;
        public string Event_Type;
        public string Event_Details;
        public string Status;
        public DateTime Occurrence_Date;
    }

    public class Review_Transformation
    {
        public int Temp_Item_ID;
        public int Item_ID;
        public string Item_Name;
        public int Category_ID;
        public string Description;
        public string Location;
        public DateTime Date;
        public int User_ID;
        public string Image_URL;
        public string Status;
        public string Process_Type;
        public DateTime Submission_Date;
        public string Review_Comments;
    }

    public class Notification_Logs
    {
        public int Notification_ID;
        public int User_ID;
        public string Notification_Type;
        public DateTime Send_Date;
        public string Status;
    }

    public class User_Activity_Logs
    {
        public int Activity_Log_ID;
        public int User_ID;
        public string Action_Type;
        public DateTime Occurrence_Time;
    }

    public class Recommendation_Logs
    {
        public int Log_ID;
        public int User_ID;
        public string Recommendation_Type;
        public DateTime Recommendation_Time;
        public string User_Feedback;
    }

    public class Transaction_Logs
    {
        public int Transaction_ID;
        public int From_User_ID;
        public int To_User_ID;
        public string Item_ID;
        public double Amount;
        public string Transaction_Type;
        public string Status;
        public DateTime StartTime;
        public DateTime FinishTime;
    }

}
