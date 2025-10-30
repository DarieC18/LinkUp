using LinkUp.Application.Services.Social;

namespace LinkUp.Infrastructure.Common
{
    public sealed class SystemClock : IDateTime
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
