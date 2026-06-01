 

namespace E_Commerce_API.Reposatory.Interface
{
    public interface IOrderRepo
    { 
             Task AddOrder(Order order);

             Task<Order> GetOrderById(int orderId, CancellationToken ct = default);

             Task<List<Order>> GetOrdersByUserId(string userId, CancellationToken ct = default);
             Task<List<Order>> GetAllOrders(CancellationToken ct = default);

             Task UpdateOrderStatus(int orderId, OrderStatus status);
            Task DeleteOrder (int  orderId);

            
        
    }
}
