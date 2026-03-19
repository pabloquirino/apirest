using ApiRest.Domain.Enums;

namespace ApiRest.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public UserRole Role { get; private set; } = UserRole.Customer;
        public bool IsActive { get; private set; } = true;

        // Navegação EF Core
        private readonly List<Order> _orders = [];
        public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();

        private readonly List<RefreshToken> _refreshTokens = [];
        public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

        // EF Core precisa de construtor sem parâmetros
        private User() { }

        public static User Create(string name, string email, string passwordHash,
                                   UserRole role = UserRole.Customer)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentException.ThrowIfNullOrWhiteSpace(email);
            ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);

            return new User
            {
                Name = name.Trim(),
                Email = email.Trim().ToLowerInvariant(),
                PasswordHash = passwordHash,
                Role = role
            };
        }

        public void UpdateProfile(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            Name = name.Trim();
            Touch();
        }

        public void ChangePassword(string newPasswordHash)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(newPasswordHash);
            PasswordHash = newPasswordHash;
            Touch();
        }

        public void Deactivate() { IsActive = false; Touch(); }
        public void Activate() { IsActive = true; Touch(); }
        public void SetAdmin() { Role = UserRole.Admin; Touch(); }
    }
}