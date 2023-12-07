using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using WebBanBalo.Data;
using WebBanBalo.Dto;
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
        [HttpGet("{receiverId}")]
        [Authorize]

        public async Task<ActionResult<IEnumerable<Message>>> GetMessages( int receiverId)
        {
            var senderId = int.Parse(User.FindFirst("Id").Value);

            var messages = await _context.Message
                .Where(m =>
                    (m.SenderUserId == senderId && m.ReceiverUserId == receiverId) ||
                    (m.SenderUserId == receiverId && m.ReceiverUserId == senderId))
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            return messages;
        }
        [HttpGet("userList")]
        [Authorize]
        public List<RecentChatUserDto> GetRecentChatUsers()
        {
            var userId = int.Parse(User.FindFirst("Id").Value);

            var recentChatUsers = _context.Message
                   .Where(m => m.SenderUserId == userId || m.ReceiverUserId == userId)
                   .GroupBy(m => m.SenderUserId == userId ? m.ReceiverUserId : m.SenderUserId)
                   .Select(g => new
                   {
                       UserId = g.Key,
                       LastMessage = g.OrderByDescending(m => m.Timestamp).FirstOrDefault()
                   })
                   .ToList();


            var userDtos = new List<RecentChatUserDto>();

            foreach (var chatUser in recentChatUsers)
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == chatUser.UserId);

                if (user != null && chatUser.LastMessage != null)
                {
                    var userDto = new RecentChatUserDto
                    {
                        UserId = user.Id,
                        UserName = user.UserName,
                        Image= user.Image,
                        LastMessageContent = chatUser.LastMessage.Content,
                        LastMessageSentTimeString = chatUser.LastMessage.Timestamp.ToString("HH:mm:ss dd/MM/yyyy")
                    };

                    userDtos.Add(userDto);
                }
            }

            return userDtos;
        }



    }
}
