using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebBanBalo.Data;
using WebBanBalo.Model;
using Newtonsoft.Json;
using WebBanBalo.Interface;

namespace WebBanBalo.HubService
{
    [Authorize]
    public class MessageService: Hub, IMessageService
    {
        private static Dictionary<string, List<string>> _connection = new Dictionary<string, List<string>>();
        private readonly DataContext _dbContext;
        private readonly IHubContext<MessageService> _hubContext;

        public MessageService(DataContext dataContext, IHubContext<MessageService> hubContext)
        {
            _dbContext = dataContext;
            _hubContext = hubContext;


        }
        public override Task OnConnectedAsync()
        {

            if (Context.User.Identity.IsAuthenticated)
            {
                // Truy xuất userId từ ClaimsPrincipal
                var userId = Context.User.FindFirst("Id")?.Value;

                if (!string.IsNullOrEmpty(userId))
                {
                    lock (_connection)
                    {
                        if (!_connection.ContainsKey(userId))
                        {
                            _connection[userId] = new List<string>();
                        }

                        _connection[userId].Add(Context.ConnectionId);
                    }
                    
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
                var messageSQL = new Message() { Content = message, Timestamp = DateTime.Now, ReceiverUserId = receiverUserId, SenderUserId = int.Parse(Context.User.FindFirst("Id").Value) };

                if (_connection.TryGetValue(receiverUserId.ToString(), out List<string> connectionIds))
                {
                    foreach (var connectionId in connectionIds)
                    {
                        await Clients.Client(connectionId).SendAsync("ReceiveMessage",  JsonConvert.SerializeObject(messageSQL));
                    }
                   
                }
                if ((connectionIds != null && !connectionIds.Contains(Context.ConnectionId)) || connectionIds== null)
                {
                    await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", JsonConvert.SerializeObject(messageSQL));

                } 
                
                _dbContext.Message.Add(messageSQL);
                _dbContext.SaveChanges();


               
            }
            catch
            {
                await Clients.Caller.SendAsync("ErrorMessage", "Server Error.");

            }
        }
        
        public async Task SendNotificatioṇ̣(string user,string message)
        {
            try
            {
                var notification = new Notification()
                {
                    Message = message,
                    CreatedAt = DateTime.Now,
                };
                
            }
            catch
            {
                await Clients.Caller.SendAsync("ErrorMessage", "Server Error.");

            }
        }

        public async Task LogoutNow(int userId)
        {
            if (_connection.TryGetValue(userId.ToString(), out List<string> connectionIds))
            {
                foreach (var connectionId in connectionIds)
                {
                    await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveLogout");
                }
            }
        }
    }
    
        
    
}
