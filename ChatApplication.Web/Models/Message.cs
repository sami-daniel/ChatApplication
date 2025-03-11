namespace ChatApplication.Web.Models
{
    public class Message
    {
        public int MessageID { get; set; }

        public string MessageContent { get; set; } = null!;

        public string MessageSender { get; set; } = null!;

        public DateTime MessageDate { get; set; }
    }
}