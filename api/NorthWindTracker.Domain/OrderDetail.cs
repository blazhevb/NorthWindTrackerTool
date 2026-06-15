namespace NorthWindTracker.Domain;

public class OrderDetail
{
    public int OrderId { get; init; }
    public int ProductId { get; init; }
    public decimal UnitPrice { get; init; }
    public short Quantity { get; init; }
    public float Discount { get; init; }

    public Product Product { get; init; } = null!;
}
