// 该C#文件里存储的是数据库表转化的数据类
<<<<<<< HEAD
using System.Text.Json.Serialization;

=======
>>>>>>> origin/LuChengBin
namespace SQLOperation.PublicAccess.Utilities
{
    public class Lost_Item
    {
<<<<<<< HEAD
        public int Item_ID;
        public string Item_Name;
        public string Category_ID;
        public string Description;
        public string Lost_Location;
        public int Lost_Date;
        public int User_ID;
        public string Lost_Status;
        public int Review_Status;
=======
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
>>>>>>> origin/LuChengBin
    }

    public class Found_Item
    {
        public int Item_ID;
        public string Item_Name;
<<<<<<< HEAD
        public string Category_ID;
        public string Description;
        public string Found_Location;
        public int Found_Date;
=======
        public int Category_ID;
        public string Description;
        public string Found_Location;
        public DateTime Found_Date;
>>>>>>> origin/LuChengBin
        public int User_ID;
        public string Match_Status;
        public int Review_Status;
    }

    public class Reward_Offers
    {
        public int User_ID;
<<<<<<< HEAD
        public int Item_ID;
        public string Reward_Amount;
=======
        public string Item_ID;
        public int Reward_Amount;
>>>>>>> origin/LuChengBin
        public string Status;
        public DateTime Release_Date;// 原来是int
        public DateTime Deadline;// 原来是int
    }
    public class Item_Status_History
    {
        public int History_ID;
        public int Change_Date;
        public int Item_ID;
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

<<<<<<< HEAD
    public class Item_Images
=======
    /*
     * public class Item_Images
>>>>>>> origin/LuChengBin
    {
        public int Image_ID;
        public string Image_URL;
        public int Item_ID;
        public string Description;
<<<<<<< HEAD
        public int Review_Status;
    }
=======
    }
    */
>>>>>>> origin/LuChengBin

    public class Item_Claim_Processes
    {
        public int Process_ID;
        public int Item_ID;
        public int Claimant_User_ID;
        public string Status;
        public DateTime Application_Date;// 原来是int
    }

    public class Match_Records
    {
        public int Record_ID;
        public int Lost_Item_ID;
        public int Found_Item_ID;
        public int Match_Date;
        public string Processing_Status;
    }

    public class Item_Exchanges
    {
        public int Exchange_ID;
        public int Lost_Item_ID;
        public int Found_Item_ID;
        public int Initiator_User_ID;
        public string Transaction_Type;
        public int Responder_User_ID;
        public string Exchange_Status;
        public DateTime Creation_Time;// 原来是int
    }

    public class Item_Return_Agreements
    {
        public int Agreement_ID;
        public int Item_ID;
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
        public int VIP_Member_ID;
        public int User_ID;
        public string Status;
        public DateTime VIP_Start_Date;
        public DateTime VIP_End_Date;
    }

    public class VIP_Orders
    {
        public int Order_ID;
        public int User_ID;
        public double Total_Amount;
        public int Point_Return;
        public DateTime Order_Time;
        public int Recharge_Time;
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
        public int Ad_ID;
        public string Ad_Content;
        public byte[] Ad_Picture;
        public string Ad_URL;
        public string Ad_Type;
        public DateTime VIP_Start_Date;
        public DateTime VIP_End_Date;
    }

    public class Ad_Click_Statistics
    {
        public int Ad_ID;
        public int User_ID;
        public DateTime Time_Stamp;
        public string IP_Address;
    }

    public class Ad_Show_Statistics
    {
        public int Ad_ID;
        public int User_ID;
        public DateTime Time_Stamp;
    }

    public class Admin_Edit_Ad
    {
        public int Admin_ID;
        public int Ad_ID;
        public DateTime Edit_Time_Stamp;
        public string Edit_Type;
        public string Edit_Content;
    }

    public class Item_Comments//
    {
        public int Comment_ID;
        public int Item_ID;
        public int User_ID;
        public string Content;
        public DateTime Date;
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
<<<<<<< HEAD
        [JsonPropertyName("USER_ID")]
        public int User_ID { get; set; }

        [JsonPropertyName("USER_NAME")]
        public string User_Name { get; set; }

        [JsonPropertyName("PASSWORD_")]
        public string Password { get; set; }

        [JsonPropertyName("CONTACT")]
        public string Contact { get; set; }
=======
        public int User_ID;
        public string User_Name;
        public string Password;
        public string Contact;
        public string Status;
>>>>>>> origin/LuChengBin
    }

    public class User_Subscriptions
    {
        public int Subscription_ID;
        public int User_ID;
        public string Subsciption_Type;
        public string Subsciption_Status;
        public DateTime Release_Date;// 原来是int
    }

    public class User_Activity
    {
        public int Activity_ID;
        public int User_ID;
        public int Activity_Type;
        public int Score;
        public DateTime Date;
    }

    public class User_Points
    {
        public int User_ID;
        public int Points;
    }

    public class User_Messages
    {
        public int Message_ID;
        public int Sender_User_ID;
        public int Receiver_User_ID;
        public string Message_Content;// SQL里是CLOB
        public string Read_Status;
    }

    public class User_Posts
    {
        public int Post_ID;
        public int User_ID;
        public string Content;
        public DateTime Post_Date;
        public int Popularity;
    }

    public class User_Relationships
    {
        public int Relation_ID;
        public int User_ID1;
        public int User_ID2;
        public string Relation_Type;
        public DateTime Established_Date;
    }

    public class Follow_List
    {
        public int Follow_ID;
        public int Follower_User_ID;
        public int Followed_User_ID;
        public DateTime Follow_Date;
    }

    public class Questions
    {
        public int Question_ID;
        public int Item_ID;
        public int User_ID;
        public string Question_Content;
        public DateTime Question_Time;
        public int Subscriber_ID;
    }

    public class Answers
    {
        public int Answer_ID;
        public int Question_ID;
        public int User_ID;
        public string Answer_Content;
        public DateTime Answer_Date;
    }

    public class Auth_Info
    {
        public int User_ID;
        public string Auth_Status;
        public DateTime Auth_Date;
        public string Status;
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
        public int Item_ID;
        public double Amount;
        public string Transaction_Type;
        public string Status;
    }
    public class UserInfo
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }



}
