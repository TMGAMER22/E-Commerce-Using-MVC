using Store.Models;

namespace Store.Chat.Models
{
    public class Conversation
    {
        public int Id{ get; set; }
        public string ClientId{ get; set; }
        public ApplicationUser Client { get; set; }
        public string CompanyId{ get; set; }
        public ApplicationUser Company { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Message> Messages { get; set; }
    }
}
