using ApiRest.Domain.Exceptions;

namespace ApiRest.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public decimal Price { get; private set; }
        public int Stock { get; private set; }
        public bool IsActive { get; private set; } = true;

        private Product() { }

        public static Product Create(string name, string description,
                                       decimal price, int stock)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            if (price < 0) throw new DomainException("Price cannot be negative.");
            if (stock < 0) throw new DomainException("Stock cannot be negative.");

            return new()
            {
                Name = name.Trim(),
                Description = description.Trim(),
                Price = price,
                Stock = stock
            };
        }

        public void Update(string name, string description, decimal price)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            if (price < 0) throw new DomainException("Price cannot be negative.");
            Name = name.Trim(); Description = description.Trim(); Price = price;
            Touch();
        }

        public void AddStock(int quantity)
        {
            if (quantity <= 0) throw new DomainException("Quantity must be positive.");
            Stock += quantity; Touch();
        }

        public void DeductStock(int quantity)
        {
            if (quantity <= 0) throw new DomainException("Quantity must be positive.");
            if (Stock < quantity) throw new DomainException($"Insufficient stock. Available: {Stock}.");
            Stock -= quantity; Touch();
        }

        public void Deactivate() { IsActive = false; Touch(); }
    }
}