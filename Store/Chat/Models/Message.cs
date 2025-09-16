using Store.Models;

namespace Store.Chat.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; }
        public string SenderId { get; set; }
        public ApplicationUser Sender { get; set; }
        public string MessageText { get; set; }
        public DateTime SendAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; }
    }
}
