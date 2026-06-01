 
namespace E_Commerce_API.Service.Interface
{
    public interface IOrderService
    {
         Task<ResponseOrderDTO> Checkout(CheckoutDTO checkDTO, CancellationToken ct = default);

        // جلب كل طلبات مستخدم
        Task<List<ResponseOrderDTO>> GetOrdersByUser(CancellationToken ct = default);

        // جلب طلب واحد
        Task<ResponseOrderDTO> GetOrderById(int orderId, CancellationToken ct = default);
        Task DeleteOrderByID(int orderId);

        // جلب كل الطلبات (Admin)
        Task<List<ResponseOrderDTO>> GetAllOrders(CancellationToken ct = default);

        // تغيير حالة طلب (Admin)
        Task UpdateOrderStatus(int orderId, OrderStatus status);
    }
}

