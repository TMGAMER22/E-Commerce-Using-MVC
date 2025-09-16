//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Store.Chat.Models;

//namespace Store.Controllers
//{
//    public class ConversationController : Controller
//    {
//        private readonly MyContext context;

//        public ConversationController(MyContext context)
//        {
//            this.context = context;
//        }


//        public async Task<IActionResult> Index()
//        {
//            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

//            if (userId == null) return Unauthorized();

//            var conversations = await context.Conversations
//                .AsNoTracking()
//                .Include(c => c.Client)
//                .Include(c => c.Company)
//                .Where(c => c.ClientId == userId || c.CompanyId == userId)
//                .ToListAsync();

//            return View(conversations);
//        }


//        public async Task<IActionResult> Chat(int id)
//        {
//            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

//            if (userId == null) return Unauthorized();

//            var conversation = await context.Conversations
//                .Include(c => c.Messages)
//                .Include(c => c.Client)
//                .Include(c => c.Company)
//                .FirstOrDefaultAsync(c => c.Id == id);

//            if (conversation == null) return NotFound();

//            // تأكيد إن اليوزر طرف في المحادثة
//            if (conversation.ClientId != userId && conversation.CompanyId != userId)
//                return Forbid();

//            return View(conversation);
//        }

//        [HttpGet]
//        public async Task<IActionResult> StartConversation(string companyId)
//        {
//            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

//            if (userId == null) return Unauthorized();

//            // هل فيه محادثة موجودة بينهم؟
//            var conversation = await context.Conversations
//                .FirstOrDefaultAsync(c =>
//                    (c.ClientId == userId && c.CompanyId == companyId) ||
//                    (c.ClientId == companyId && c.CompanyId == userId));

//            // لو مفيش، نعمل واحدة جديدة
//            if (conversation == null)
//            {
//                conversation = new Conversation
//                {
//                    ClientId = userId,
//                    CompanyId = companyId,
//                    Messages = new List<Message>()
//                };

//                context.Conversations.Add(conversation);
//                await context.SaveChangesAsync();
//            }

//            // ودّيه للشات
//            return RedirectToAction("Chat", new { id = conversation.Id });
//        }
//    }

//}
