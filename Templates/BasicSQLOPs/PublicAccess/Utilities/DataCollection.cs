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
        public int Item_ID { get; set; }
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

    public class Users
    {
        public int User_ID { get; set; }
        public string User_Name { get; set; }

    }

}
