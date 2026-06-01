 
namespace E_Commerce_API.Reposatory.Implementation
{
    public class OrderRepo : IOrderRepo
    {

        private readonly Application _db;

        public OrderRepo(Application db)
        {
            _db = db;
        }

        public async Task AddOrder(Order order)
        {
            await _db.Orders.AddAsync(order);
        }

        public async Task<Order> GetOrderById(int orderId, CancellationToken ct = default)
        {
            return await _db.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(a => a.Products)
                .Include(a => a.User)
                .FirstOrDefaultAsync(o => o.Id == orderId, ct);
        }

        public async Task<List<Order>> GetOrdersByUserId(string userId, CancellationToken ct = default)
        {
            return await _db.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(a => a.Products)
                .Include(a => a.User)
                .Where(o => o.UserId == userId)
                .ToListAsync(ct);
        }

        public async Task<List<Order>> GetAllOrders(CancellationToken ct = default)
        {
            return await _db.Orders
                .Include(o => o.OrderItems)
                .Include(a => a.User)
                .ToListAsync(ct);
        }

        public async Task UpdateOrderStatus(int orderId, OrderStatus status)
        {
            var order = await _db.Orders.FindAsync(orderId);
             order.Status = status;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteOrder(int orderId)
        {
             var order = await _db.Orders.FindAsync(orderId);

            // 2. التأكد أن الأوردر موجود
            if (order == null)
                throw new KeyNotFoundException("Order ID Not Found");
             
                 _db.Orders.Remove(order);
                 await _db.SaveChangesAsync();
            
        }
    }
    }
