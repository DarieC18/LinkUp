using LinkUp.Application.DTOs.Social;

namespace LinkUp.Application.ViewModels.Friends
{
    public sealed class FriendRequestsIndexVm
    {
        public List<FriendRequestListItemDto> Received { get; set; } = new();
        public List<FriendRequestListItemDto> Sent { get; set; } = new();
    }
}
