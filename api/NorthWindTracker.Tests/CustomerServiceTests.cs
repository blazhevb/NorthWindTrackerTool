using NorthWindTracker.Application.DTOs;
using NorthWindTracker.Application.Interfaces;
using NorthWindTracker.Application.Services;
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
        var expected = new List<CustomerSummaryDto>
        {
            new("ALFKI", "Alfreds Futterkiste", 6),
            new("ANATR", "Ana Trujillo Emparedados", 4)
        };
        _repository.GetAllAsync(null).Returns(expected);

        var result = await _sut.GetAllAsync(null);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public async Task GetAllAsync_PassesNameFilterToRepository()
    {
        _repository.GetAllAsync("alfreds").Returns([]);

        await _sut.GetAllAsync("alfreds");

        await _repository.Received(1).GetAllAsync("alfreds");
    }

    [Test]
    public async Task GetByIdAsync_ReturnsCustomerDetail_WhenCustomerExists()
    {
        var orders = new List<OrderSummaryDto>
        {
            new(10643, new DateTime(1997, 8, 25), 814.50m, 3)
        };
        var expected = new CustomerDetailDto("ALFKI", "Alfreds Futterkiste", "Maria Anders", "Berlin", "Germany", orders);
        _repository.GetByIdAsync("ALFKI").Returns(expected);

        var result = await _sut.GetByIdAsync("ALFKI");

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public async Task GetByIdAsync_ReturnsNull_WhenCustomerDoesNotExist()
    {
        _repository.GetByIdAsync("XXXXX").Returns((CustomerDetailDto?)null);

        var result = await _sut.GetByIdAsync("XXXXX");

        Assert.That(result, Is.Null);
    }
}
