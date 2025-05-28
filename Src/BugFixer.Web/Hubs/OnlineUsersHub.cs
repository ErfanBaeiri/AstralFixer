using BugFixer.Application.Extensions;
using BugFixer.Application.Services.Interfaces;
using BugFixer.Domain.ViewModels.Admin.OnlineUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace BugFixer.Web.Hubs
{
    [Authorize]
    public class OnlineUsersHub : Hub
    {
        #region Ctor
        private readonly IUserService _userService;
        public OnlineUsersHub(IUserService userService)
        {
            _userService = userService;
        }
        #endregion

        private static readonly Dictionary<long, string> OnlineUsersList = new Dictionary<long, string>();

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.GetUserId();

            if (userId == null) return;

            if (OnlineUsersList.ContainsKey(userId.Value)) return;

            var user = await _userService.GetUserByIdAsync(userId.Value);

            if (user == null) return;

            var onlineUserViewModel = new OnlineUsersViewModel
            {
                ConnectedDate = $"{DateTime.Now.ToPersianDate()} - {DateTime.Now:HH:mm:ss}",
                DisplayName = user.GetUserDisplayName(),
                UserId = userId.Value.ToString()
            };

            OnlineUsersList.Add(userId.Value, JsonConvert.SerializeObject(onlineUserViewModel));

            await Clients.All.SendAsync("NewUserConnected", onlineUserViewModel);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {

            var userId = Context.User?.GetUserId();

            if (userId == null) return;

            if (!OnlineUsersList.ContainsKey(userId.Value)) return;

            OnlineUsersList.Remove(userId.Value);

            await Clients.All.SendAsync("NewUserDisConnected", userId.Value.ToString());

            await base.OnDisconnectedAsync(exception);
        }
    }
}
