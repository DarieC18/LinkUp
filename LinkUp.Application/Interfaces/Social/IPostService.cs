using LinkUp.Application.DTOs.Social;

namespace LinkUp.Application.Interfaces.Social
{

    public interface IPostService
    {
        Task<Guid> CreateAsync(CreatePostRequest request);
        Task<PagedResult<PostFeedItemDto>> GetFeedAsync(GetFeedRequest request);
        Task<ToggleReactionResult> ToggleReactionAsync(ToggleReactionRequest request);
        Task<PostForEditDto> GetForEditAsync(Guid postId, string userId);
        Task EditAsync(EditPostRequest request);
        Task DeleteAsync(DeletePostRequest request);
        Task<PagedResult<PostFeedItemDto>> GetFeedByFriendsAsync(string userId, int page, int pageSize, CancellationToken ct = default);

    }
}
