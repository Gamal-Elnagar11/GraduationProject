 namespace E_Commerce_API.Service.Implementation
{
    public class CartService : ICartService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMapper mapper;

        public CartService(UserManager<User> userManager, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _contextAccessor = httpContextAccessor;
            this.mapper = mapper;
        }

        private string GetUserId(CancellationToken ct = default)
        {
            var user = _contextAccessor.HttpContext?.User;

             if (user == null || !user.Identity.IsAuthenticated)
                throw new UnauthorizedAccessException("User is not authenticated. Please log in.");

             var userId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                         ?? user.FindFirst("sub")?.Value;

             if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User ID claim is missing from the token.");

            return userId;
        }


        public async Task<ResponseCartDTO> GetCartByUserId(CancellationToken ct = default)
        {
            var userid = GetUserId(ct);
             var result = await _unitOfWork.CartRepo.GetCartByUserId(userid,ct);

            if (result == null)
                throw new KeyNotFoundException("Cart not found for this user ID");

            var map = mapper.Map<ResponseCartDTO>(result);
            return map;
        }

        public async Task<Cart> GetOrCreateCart(CancellationToken ct = default)
        {
            var userid = GetUserId(ct);
            if (string.IsNullOrEmpty(userid))
                throw new UnauthorizedAccessException("User must be logged in.");

            var cart = await _unitOfWork.CartRepo.GetCartByUserId(userid,ct);
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userid,
                    CartItems = new List<CartItem>() // تجهيز اللستة فاضية فوراً منعاً لأي نال
                };

                await _unitOfWork.CartRepo.AddCart(cart);
                await _unitOfWork.CompleteAsync();
            }
             return cart;
        }

        public async Task ClearCart(Cart cart)
        {
            if (cart == null)
                throw new ArgumentException("Cart Not Found");

            if (cart.CartItems != null && cart.CartItems.Any())
            {
                 _unitOfWork.CartRepo.DeleteCartItems(cart.CartItems);
                cart.CartItems.Clear();  
            }

            await _unitOfWork.CompleteAsync();
        }


        public async Task AddItemCart(int productid, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero");

            // 1. التعديل الجوهري: التشيك على وجود المنتج في الداتا بيز أولاً
            var product = await _unitOfWork.ProductRepo.GetProductsByIdAsync(productid);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {productid} does not exist.");

            // 2. بنجيب أو نكريت كارت اليوزر
            var cart = await GetOrCreateCart();

            // 3. بندور على المنتج جوه الكارت
            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productid);

            if (existingItem != null)
            {
                // لو موجود، بنزود الكمية
                existingItem.Quantity += quantity;
            }
            else
            {
                // لو مش موجود، بنخلق Item جديد ونضيفه للكارت
                var newItem = new CartItem
                {
                    ProductId = productid,
                    Quantity = quantity,
                    CartId = cart.Id
                };

                cart.CartItems.Add(newItem);
            }

            // 4. سيف مرة واحدة في الآخر لكل الحالات بروقان ونظافة
            await _unitOfWork.CompleteAsync();
        }


        public async Task UpdateItemCartQuantity(int productid, int newquantity)
        {
            if (newquantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero");

            // 1. بنجيب كارت اليوزر الحالي من الـ Token
            var cart = await GetOrCreateCart();

            // 2. بندور على المنتج جوه الكارت
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productid);

            if (cartItem == null)
                throw new ArgumentException("Product not found in cart");

            // 3. بنعدل الكمية ونحفظ في الداتا بيز
            cartItem.Quantity = newquantity;
            await _unitOfWork.CompleteAsync();

            // مفيش سطر return هنا خالص كدة زي ما اتفقنا!
        }


        public async Task DeleteItemFromCart(int productid)
        {
            // 1. بنجيب كارت اليوزر الحالي بأمان من الـ Token (من غير ما نبعته في البارامترز)
            var cart = await GetOrCreateCart();

            // 2. بندور على الـ Item جوه الكارت
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productid);

            if (cartItem == null)
                throw new ArgumentException("Product not found in cart");

            // 3. بنمسح الـ Item صراحة من جدول الـ CartItems في الداتا بيز (باستخدام ميثود الريبو اللي زودناها)
            _unitOfWork.CartRepo.DeleteCartItems(new List<CartItem> { cartItem });

            // 4. بنشيله من اللستة في الميموري ونعمل حفظ
            cart.CartItems.Remove(cartItem);
            await _unitOfWork.CompleteAsync();
        }






    }
}
