namespace NorthWindTracker.Domain;

public class Order
{
    public int OrderId { get; init; }
    public DateTime? OrderDate { get; init; }

    public ICollection<OrderDetail> OrderDetails { get; init; } = [];
}
