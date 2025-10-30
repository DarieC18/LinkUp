using LinkUp.Application.DTOs.Social;

namespace LinkUp.Application.ViewModels.Friends
{
    public sealed class FriendsIndexVm
    {
        public List<FriendListItemDto> Friends { get; set; } = new();
        public List<PostFeedItemDto> Posts { get; set; } = new();
    }
}
