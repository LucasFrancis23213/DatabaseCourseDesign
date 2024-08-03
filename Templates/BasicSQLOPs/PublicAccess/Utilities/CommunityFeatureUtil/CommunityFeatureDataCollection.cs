namespace SQLOperation.PublicAccess.Utilities
{
    public class Conversation_User_Message
    {
        public Users Users;
        public Conversation Conversation;

    }

    public class Conversation {
        public int Sender_User_ID {  get; set; }
        public int Receiver_User_ID { get; set; }
        public string Message_Content {  get; set; }
        public DateTime Last_Message_Time {  get; set; }
        public int Unread_Count {  get; set; }
    }

    public class AdvertisementsDetails {
        public Advertisements Advertisements { get; set; }

        public int Click_Count {  get; set; }

        public int Show_Count { get; set; }
    }




}