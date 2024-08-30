namespace SQLOperation.PublicAccess.Utilities
{
    public class Conversation_User_Message
    {
        public Users Users;
        public Conversation Conversation;

    }

    public class Conversation {
        public string Message_Content {  get; set; }
        public DateTime Last_Message_Time {  get; set; }
        public int Unread_Count {  get; set; }
    }

    



}