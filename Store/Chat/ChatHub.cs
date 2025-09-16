//using Microsoft.AspNetCore.SignalR;
//using Store.Chat.Models;

//namespace Store.Chat
//{
//    public class ChatHub:Hub
//    {
//        private readonly MyContext context;

//        public ChatHub(MyContext _context)
//        {
//            context = _context;
//        }
//        public async Task JoinConversation(string conversationId)
//        {
//            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
//        }

//        public async Task SendMessage(string conversationId, string senderId, string message)
//        {
//            var msg = new Message
//            {
//                ConversationId = int.Parse(conversationId),
//                SenderId = senderId,
//                MessageText = message,
//                SendAt = DateTime.UtcNow
//            };

//            context.Messages.Add(msg);
//            await context.SaveChangesAsync();

//            await Clients.Group(conversationId).SendAsync("ReceiveMessage", senderId, message);
//        }
//    }
//}
