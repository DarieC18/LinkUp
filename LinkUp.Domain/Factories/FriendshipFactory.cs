public static class FriendshipFactory
{
    public static LinkUp.Domain.Entities.Social.Friendship Create(string userIdA, string userIdB)
    {
        var (u1, u2) = string.CompareOrdinal(userIdA, userIdB) < 0
            ? (userIdA, userIdB)
            : (userIdB, userIdA);

        return new LinkUp.Domain.Entities.Social.Friendship
        {
            UserId1 = u1,
            UserId2 = u2
        };
    }
}
