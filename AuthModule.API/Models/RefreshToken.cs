namespace AuthModule.API.Models
{
    public class RefreshToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; } = default!;
        public string TokenHash { get; set; } = default!;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset ExpiresAt { get; set; }
        public DateTimeOffset? RevokedAt { get; set; }
        public Guid? ReplacedByTokenId { get; set; }
        public string? DeviceInfo { get; set; }
        public string? IpAddress { get; set; }


        public bool IsActive => RevokedAt == null && DateTimeOffset.UtcNow < ExpiresAt;
    }
}
