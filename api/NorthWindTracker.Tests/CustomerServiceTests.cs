using NorthWindTracker.Application.Interfaces;
using NorthWindTracker.Application.Services;
using NorthWindTracker.Domain;
using NSubstitute;
using NUnit.Framework;

namespace NorthWindTracker.Tests;

[TestFixture]
public class CustomerServiceTests
{
    private ICustomerRepository _repository = null!;
    private CustomerService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = Substitute.For<ICustomerRepository>();
        _sut = new CustomerService(_repository);
    }

    [Test]
    public async Task GetAllAsync_ReturnsAllCustomers_WhenNoFilterProvided()
    {
        var customers = new List<Customer>
        {
            new() { CustomerId = "ALFKI", CompanyName = "Alfreds Futterkiste", Orders = [new() { OrderId = 1 }, new() { OrderId = 2 }] },
            new() { CustomerId = "ANATR", CompanyName = "Ana Trujillo Emparedados", Orders = [new() { OrderId = 3 }] }
        };
        _repository.GetAllAsync(null).Returns(customers);

        var result = (await _sut.GetAllAsync(null)).ToList();

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0].CustomerId, Is.EqualTo("ALFKI"));
        Assert.That(result[0].OrderCount, Is.EqualTo(2));
        Assert.That(result[1].CustomerId, Is.EqualTo("ANATR"));
        Assert.That(result[1].OrderCount, Is.EqualTo(1));
    }

    [Test]
    public async Task GetAllAsync_PassesNameFilterToRepository()
    {
        _repository.GetAllAsync("alfreds").Returns([]);

        await _sut.GetAllAsync("alfreds");

        await _repository.Received(1).GetAllAsync("alfreds");
    }

    [Test]
    public async Task GetByIdAsync_ReturnsCorrectOrderTotals_WhenCustomerExists()
    {
        var customer = new Customer
        {
            CustomerId = "ALFKI",
            CompanyName = "Alfreds Futterkiste",
            ContactName = "Maria Anders",
            City = "Berlin",
            Country = "Germany",
            Orders =
            [
                new Order
                {
                    OrderId = 10643,
                    OrderDate = new DateTime(1997, 8, 25),
                    OrderDetails =
                    [
                        new OrderDetail { ProductId = 1, UnitPrice = 10m, Quantity = 2, Discount = 0f },
                        new OrderDetail { ProductId = 2, UnitPrice = 5m, Quantity = 4, Discount = 0.1f }
                    ]
                }
            ]
        };
        _repository.GetByIdAsync("ALFKI").Returns(customer);

        var result = await _sut.GetByIdAsync("ALFKI");

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Orders.First().OrderId, Is.EqualTo(10643));
        Assert.That(result.Orders.First().ProductCount, Is.EqualTo(2));
        Assert.That(result.Orders.First().TotalValue, Is.EqualTo(10m * 2 + 5m * 4 * (decimal)(1 - 0.1f)).Within(0.001m));
    }

    [Test]
    public async Task GetByIdAsync_ReturnsNull_WhenCustomerDoesNotExist()
    {
        _repository.GetByIdAsync("XXXXX").Returns((Customer?)null);

        var result = await _sut.GetByIdAsync("XXXXX");

        Assert.That(result, Is.Null);
    }
}
