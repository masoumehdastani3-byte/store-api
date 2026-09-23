using System;

namespace Api.Models
{
    public class Telegramobject
    {
        public bool ok { get; set; }
        public string error_code { get; set; }
        public string description { get; set; }

        public Result[] result { get; set; }
    }

    public class Result
    {
        public int update_id { get; set; }
        public Message message { get; set; }
        public My_Chat_Member my_chat_member { get; set; }
    }

    public class Message
    {
        public int message_id { get; set; }
        public From from { get; set; }
        public Chat chat { get; set; }
        public int date { get; set; }
        public string text { get; set; }
        public Entity[] entities { get; set; }
        public New_Chat_Participant new_chat_participant { get; set; }
        public New_Chat_Member new_chat_member { get; set; }
        public Object new_chat_members { get; set; }
    }




    public class New_Chat_Participant
    {
        public int id { get; set; }
        public bool is_bot { get; set; }
        public string first_name { get; set; }
        public string username { get; set; }
    }



    public class Entity
    {
        public int offset { get; set; }
        public int length { get; set; }
        public string type { get; set; }
    }





    //+++++++++++++++++++++++++++++++++++++++++++++



    public class My_Chat_Member
    {
        public Chat chat { get; set; }
        public From from { get; set; }
        public int date { get; set; }
        public Old_Chat_Member old_chat_member { get; set; }
        public New_Chat_Member new_chat_member { get; set; }
    }
    public class Chat
    {
        public long id { get; set; }
        public string first_name { get; set; }
        public string username { get; set; }
        public string type { get; set; }
        public string title { get; set; }
    }



    public class From
    {
        public int id { get; set; }
        public bool is_bot { get; set; }
        public string first_name { get; set; }
        public string username { get; set; }
        public string language_code { get; set; }
    }

    public class Old_Chat_Member
    {
        public User user { get; set; }
        public string status { get; set; }
    }

    public class User
    {
        public int id { get; set; }
        public bool is_bot { get; set; }
        public string first_name { get; set; }
        public string username { get; set; }
    }

    public class New_Chat_Member
    {
        public User user { get; set; }
        public string status { get; set; }
        public bool? can_be_edited { get; set; }
        public bool? can_manage_chat { get; set; }
        public bool? can_change_info { get; set; }
        public bool? can_post_messages { get; set; }
        public bool? can_edit_messages { get; set; }
        public bool? can_delete_messages { get; set; }
        public bool? can_invite_users { get; set; }
        public bool? can_restrict_members { get; set; }
        public bool? can_promote_members { get; set; }
        public bool? can_manage_voice_chats { get; set; }
        public bool is_anonymous { get; set; }

        public int? id { get; set; }
        public bool? is_bot { get; set; }
        public string first_name { get; set; }
        public string username { get; set; }
    }



}