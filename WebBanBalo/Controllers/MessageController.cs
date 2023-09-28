using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using WebBanBalo.Data;
using WebBanBalo.Model;

namespace WebBanBalo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        public DataContext _context;
        public MessageController(DataContext context) {
            _context = context;
        }
        [HttpGet("{senderId}/{receiverId}")]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessages(int senderId, int receiverId)
        {
            var messages = await _context.Message
                .Where(m =>
                    (m.SenderUserId == senderId && m.ReceiverUserId == receiverId) ||
                    (m.SenderUserId == receiverId && m.ReceiverUserId == senderId))
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            return messages;
        }
        [HttpGet("userList")]
        public List<Users> GetRecentChatUsers(int userId)
        {
            var recentChatUsers = _context.Message
                .Where(m => m.SenderUserId == userId || m.ReceiverUserId == userId)
                .GroupBy(m => m.SenderUserId == userId ? m.ReceiverUserId : m.SenderUserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    LastMessageSentAt = g.Max(m => m.Timestamp)
                })
                .OrderByDescending(u => u.LastMessageSentAt)
                .Select(u => u.UserId)
                .ToList();

            var users = _context.Users
                .Where(u => recentChatUsers.Contains(u.Id))
                .ToList();

            return users;
        }

    }
}
