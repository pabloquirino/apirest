namespace ApiRest.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public Guid UserId { get; private set; }
        public string Token { get; private set; } = string.Empty;
        public DateTime ExpiresAt { get; private set; }
        public bool IsRevoked { get; private set; } = false;

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsValid => !IsRevoked && !IsExpired;

        // Navegação
        public User User { get; private set; } = null!;

        private RefreshToken() { }

        public static RefreshToken Create(Guid userId, string token, int expiryDays)
            => new()
            {
                UserId = userId,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddDays(expiryDays)
            };

        public void Revoke() { IsRevoked = true; Touch(); }
    }
}