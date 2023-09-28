using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebBanBalo.Data;
using WebBanBalo.Model;

namespace WebBanBalo.HubService
{
    [Authorize]
    public class MessageService: Hub
    {
        private static Dictionary<string, List<string>> _connection = new Dictionary<string, List<string>>();
        private readonly DataContext _dbContext;
        public MessageService(DataContext dataContext)
        {
            _dbContext = dataContext;


        }
        public override Task OnConnectedAsync()
        {

            if (Context.User.Identity.IsAuthenticated)
            {
                // Truy xuất userId từ ClaimsPrincipal
                var userId = Context.User.FindFirst("Id")?.Value;

                if (!string.IsNullOrEmpty(userId))
                {
                    if (!_connection.ContainsKey(userId))
                    {
                        _connection[userId] = new List<string>();
                    }

                    _connection[userId].Add(Context.ConnectionId);
                }

                

            }

            

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            try

            {
                var userId = _connection.FirstOrDefault(x => x.Value.Contains(Context.ConnectionId)).Key;

                if (userId != null)
                {
                    _connection[userId].Remove(Context.ConnectionId);

                    if (_connection[userId].Count == 0)
                    {
                        _connection.Remove(userId);
                    }
                }
            }
            catch
            {
            }
            return base.OnDisconnectedAsync(exception);

        }
        public async Task SendMessage(int receiverUserId, string message)
        {
            try
            {

                if (_connection.TryGetValue(receiverUserId.ToString(), out List<string> connectionIds))
                {
                    // Gửi tin nhắn đến tất cả các kết nối của người nhận
                    foreach (var connectionId in connectionIds)
                    {
                        await Clients.Client(connectionId).SendAsync("ReceiveMessage", Context.User.FindFirst("UserName").Value, message);
                    }
                }
                var messageSQL = new Message() { Content= message, Timestamp=DateTime.Now, ReceiverUserId=receiverUserId,SenderUserId=int.Parse(Context.User.FindFirst("Id").Value) };
                _dbContext.Message.Add(messageSQL);


                if (!connectionIds.Contains(Context.ConnectionId))
                {
                    await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", Context.User.FindFirst("UserName").Value, message);

                }
            }
            catch
            {
                await Clients.Caller.SendAsync("ErrorMessage", "Server Error.");

            }
        }

    }
        
    
}
