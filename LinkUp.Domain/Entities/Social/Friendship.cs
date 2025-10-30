namespace LinkUp.Domain.Entities.Social
{
    public class Friendship
    {
        public string UserId1 { get; set; } = default!;
        public string UserId2 { get; set; } = default!;

        public DateTime CreatedAtUtc { get; set; }
    }
}
