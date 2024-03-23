namespace Parcel2Go.TechTest2024.UnitTests
{
    [TestClass]
    public class CheckoutTests
    {
        [DataTestMethod]
        [DataRow(new[] { "B", "B" }, 20, DisplayName = "Example 1: Multipurchase Discount Advantage")]
        [DataRow(new[] { "F", "C" }, 23, DisplayName = "Example 2: No Multipurchase Discount")]
        [DataRow(new[] { "F", "F", "B" }, 27, DisplayName = "Example 3: Mix of Discounts and No Discount")]
        [DataRow(new[] { "A" }, 10, DisplayName = "1 x A")]
        [DataRow(new[] { "A", "A" }, 20, DisplayName = "2 x A")]
        [DataRow(new[] { "A", "A", "A" }, 25, DisplayName = "3 x A")]
        [DataRow(new[] { "A", "A", "A", "A" }, 35, DisplayName = "4 x A")]
        [DataRow(new[] { "C" }, 15, DisplayName = "1 x C")]
        [DataRow(new[] { "C", "C" }, 30, DisplayName = "2 x C")]
        public void GetTotalPrice_ReturnsExpectedValue(string[] cart, int expected)
        {
            // Arrange
            var services = new Dictionary<string, Service>
                {
                    { "A", new Service(10, new SpecialOffer(3, 25)) },
                    { "B", new Service(12, new SpecialOffer(2, 20)) },
                    { "C", new Service(15) },
                    { "D", new Service(25) },
                    { "F", new Service(8, new SpecialOffer(2, 15)) }
                };
            var target = new Checkout(services);

            foreach (var service in cart)
            {
                target.Scan(service);
            }

            // Act
            var actual = target.GetTotalPrice();

            // Assert           
            Assert.AreEqual(expected, actual);
        }
    }

    internal interface ICheckout
    {
        void Scan(string service);

        int GetTotalPrice();
    }

    internal class Checkout(IDictionary<string, Service> services) : ICheckout
    {
        private readonly IDictionary<string, Service> _services = services;
        private readonly ICollection<string> _cart = [];

        public void Scan(string service)
        {
            _cart.Add(service);
        }

        public int GetTotalPrice()
        {
            var sum = 0;

            foreach (var service in _services.Where(s => _cart.Any(c => c == s.Key)))
            {
                var serviceQuantity = _cart.Count(s => s == service.Key);

                if (service.Value.SpecialOffer is null)
                {
                    sum += serviceQuantity * service.Value.StandardPrice;
                }
                else
                {
                    var specialOfferQuantity = serviceQuantity / service.Value.SpecialOffer.Quantity;
                    var standardQuantity = serviceQuantity - (specialOfferQuantity * service.Value.SpecialOffer.Quantity);

                    sum += specialOfferQuantity * service.Value.SpecialOffer.Price;
                    sum += standardQuantity * service.Value.StandardPrice;
                }
            }

            return sum;
        }
    }

    internal class Service(int standardPrice, SpecialOffer? specialOffer = null)
    {
        public int StandardPrice { get; internal set; } = standardPrice;

        public SpecialOffer? SpecialOffer { get; internal set; } = specialOffer;
    }

    internal class SpecialOffer(int quantity, int price)
    {
        public int Quantity { get; internal set; } = quantity;

        public int Price { get; internal set; } = price;
    }
}