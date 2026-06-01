 
namespace E_Commerce_API.Service.Implementation
{
    public class OrderService : IOrderService
    {
         private readonly ICartService _cartService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor; // زود ده
        private readonly IMapper mapper;
        private readonly ILogger<OrderService> logger;

        public OrderService(ICartService cartService, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IMapper mapper, ILogger<OrderService> logger)
        {
            _cartService = cartService;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            this.mapper = mapper;
            this.logger = logger;
        }




        public async Task<ResponseOrderDTO> Checkout(CheckoutDTO checkDTO, CancellationToken ct = default)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                throw new ArgumentException("User ID Not Found");

            var trans = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var cart = await _cartService.GetOrCreateCart(ct);

                if (cart?.CartItems == null || !cart.CartItems.Any())
                    throw new ArgumentException("Cart is empty");

                if (!Enum.IsDefined(typeof(Payment), checkDTO.PaymentMethod))
                    throw new ArgumentException("Invalid payment method");

                // 1. إنشاء الـ Order الأساسي وتجهيز لستة الـ Items فاضية
                var order = new Order
                {
                    UserId = userId,
                    PhoneNumber = checkDTO.PhoneNumber,
                    City = checkDTO.City,
                    Address = checkDTO.Address,
                    PaymentMethod = checkDTO.PaymentMethod,
                    Status = checkDTO.PaymentMethod == Payment.CreditCard ? OrderStatus.PendingPayment : OrderStatus.Pending,
                    DateTime = DateTime.UtcNow,
                    OrderItems = new List<OrderItem>() // بنجهز اللستة
                };

                decimal totalOrderPrice = 0;

                // 2. اللوب على عناصر الكارت: تشيك Stock + بناء الـ Order Items من داتا حقيقية
                foreach (var item in cart.CartItems)
                {
                    var product = await _unitOfWork.ProductRepo.GetProductsByIdAsync(item.ProductId,ct);

                    if (product == null)
                        throw new ArgumentException($"Product with id {item.ProductId} not found");

                    if (product.Stock < item.Quantity)
                    {
                        logger.LogWarning("This Product {Name} Not enough To Create Order. At {Time}.", product.Name, DateTime.Now);
                        throw new ArgumentException($"Not enough stock for product {product.Name}");
                    }

                    // خصم الـ Stock
                    product.Stock -= item.Quantity;

                    // بناء الـ OrderItem بأمان من الـ product اللي جاي من الـ repo مباشرة لمنع الـ Null
                    var orderItem = new OrderItem
                    {
                        ProductId = item.ProductId,
                        ProductName = product.Name, // من الـ product المضمون
                        Quantity = item.Quantity,
                        Price = product.Price,       // السعر الحالي الحقيقي
                        TotalPrice = item.Quantity * product.Price
                    };

                    order.OrderItems.Add(orderItem);
                    totalOrderPrice += orderItem.TotalPrice;
                }

                order.TotalPrice = totalOrderPrice;

                // 3. حفظ الطلب في الداتا بيز
                await _unitOfWork.OrderRepo.AddOrder(order);
                await _unitOfWork.CompleteAsync(); // بيحفظ الـ Order وبيسمع تعديلات الـ Stock كلها مرة واحدة

                // 4. تفريغ الكارت (جوه الـ Transaction عشان لو ضربت يرجع في كلامه)
                await _cartService.ClearCart(cart);

                // 5. تثبيت العملية بنجاح
                await trans.CommitAsync();
                var result = mapper.Map<ResponseOrderDTO>(order);
                logger.LogInformation("Create Order Successfuly By ID {ID}. At {Time}.", order.Id, DateTime.Now);
                 return result;
            }
            catch (Exception)
            {
                await trans.RollbackAsync();
                logger.LogError("Create Order Failed. At {Time}.",  DateTime.Now);
                throw;
            }
        }



        public async Task<List<ResponseOrderDTO>> GetOrdersByUser(CancellationToken ct = default)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                throw new ArgumentException("User ID Not Found");

            var result = await _unitOfWork.OrderRepo.GetOrdersByUserId(userId,ct);
            var map = mapper.Map<List<ResponseOrderDTO>>(result);
            return map;
        }


        public async Task<ResponseOrderDTO> GetOrderById(int orderId, CancellationToken ct = default)
        {
            var order = await _unitOfWork.OrderRepo.GetOrderById(orderId,ct);
            if (order == null)
                throw new ArgumentException("Order not found");
            var result = mapper.Map<ResponseOrderDTO>(order);
            return result;
        }

          
        public async Task<List<ResponseOrderDTO>> GetAllOrders(CancellationToken ct = default)
        {
             var result = await _unitOfWork.OrderRepo.GetAllOrders(ct);
            var map = mapper.Map<List<ResponseOrderDTO>>(result);
            return map;
        }
         
        public async Task UpdateOrderStatus(int orderId, OrderStatus stock)
        {

            var result = await GetOrderById(orderId);
            if (result == null)
                throw new ArgumentException("OrderID Not Found");

            if (stock < 0)
                throw new ArgumentException("Value must be grater than Zero");
            await _unitOfWork.OrderRepo.UpdateOrderStatus(orderId, stock);
            await _unitOfWork.CompleteAsync();

             await _unitOfWork.OrderRepo.GetOrderById(orderId);
        }

        public async Task DeleteOrderByID(int orderId)
        {
            var result = await GetOrderById(orderId);
            if (result == null)
                throw new ArgumentException("OrderID Not Found");
            logger.LogInformation("Delete Order ID {ID} Successfuly. At {Time}.", orderId, DateTime.Now);
              await _unitOfWork.OrderRepo.DeleteOrder(orderId);
        }
    }
         
 }