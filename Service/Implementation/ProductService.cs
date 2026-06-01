 
namespace E_Commerce_API.Service.Implementation
{
    public class ProductService : IProductService
    {
        private readonly IMapper mapper;
         private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductService> logger;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ProductService> logger)
        {
            _unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.logger = logger;
        }



        public async Task<ProductBaseDTO> AddProductAsync(AddProductDTO product, CancellationToken ct = default)
        {
              await _unitOfWork.BeginTransactionAsync();
            string SavedFilePath = null;
            try
            {
               
                var category = await _unitOfWork.Repositoey<Category>().GetByIdAsync(product.CategoryId,ct);
                if (category == null)
                    throw new ArgumentException("Category Id Invalid ");

                string imageUrl = null; // هنعمل متغير نشيل فيه الباص مؤقتاً

                if (product.Image != null)
                {
                   
                     var fileName = Guid.NewGuid().ToString() + Path.GetExtension(product.Image.FileName);

                    // مسار التخزين على السيرفر (wwwroot/images)
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                    // احفظ الصورة فعليًا
                    SavedFilePath = filePath;
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await product.Image.CopyToAsync(stream);
                    }

                     imageUrl = "/images/" + fileName;

                }

                var productEntity = mapper.Map<Product>(product);
                productEntity.ImageUrl = imageUrl;

                var result = await _unitOfWork.ProductRepo.AddAsync(productEntity);    //Repositoey<Product>().AddAsync(product);
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommmetTransactionAsync();
                logger.LogInformation("Add Product {Name} Successfuly. At {Time}", product.Name, DateTime.Now);
                var endresult = mapper.Map<ProductBaseDTO>(result);
                 return endresult;
            }
            catch(Exception ex)
            {
                if (!string.IsNullOrEmpty(SavedFilePath)
                   && File.Exists(SavedFilePath))
                {
                    File.Delete(SavedFilePath);
                }
                await _unitOfWork.RollebackAsync();
                logger.LogError("Add Product {Name} Failed. At {Time}", product.Name, DateTime.Now);
                throw ;
            }
        }
         
        public async Task DeleteProductAsync(int id, CancellationToken ct = default)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var result = await _unitOfWork.Repositoey<Product>().GetByIdAsync(id,ct);
                if (result == null)
                    throw new KeyNotFoundException("Product ID Not found");

                _unitOfWork.Repositoey<Product>().Delete(result);
                await _unitOfWork.CompleteAsync();

                if (!string.IsNullOrEmpty(result.ImageUrl))
                {
                    // احصل على المسار الكامل للصورة
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", result.ImageUrl.TrimStart('/'));

                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
                logger.LogInformation("Delete Product {Name} Successfuly. At {Time}", result.Name, DateTime.Now);
                await _unitOfWork.CommmetTransactionAsync();    
             }
            catch(Exception ex)
            {
                await _unitOfWork.RollebackAsync();
                logger.LogError("Delete Product ID {ID} Failed. At {Time}",id, DateTime.Now);
                throw;
            }
        }

        public async Task<ProductBaseDTO> GetProductByIdAsync(int id, CancellationToken ct = default)
        {
            // بنجيب المنتج بالـ Id وبنقوله هات معاه الـ Category من الداتا بيز فوراً
            var product = await _unitOfWork.Repositoey<Product>()
                                           .GetByIdWithIncludesAsync(id,ct, p => p.Category);

            if (product == null)
                throw new KeyNotFoundException("Product Not Found");

            var result = mapper.Map<ProductBaseDTO>(product);
            return result;
        }

        public async Task<List<ProductBaseDTO>>GetAllProductsAsync(CancellationToken ct = default)
        {
              var map = await _unitOfWork.Repositoey<Product>().GetAll(ct);
            var result = mapper.Map<List<ProductBaseDTO>>(map);
            logger.LogInformation("get all product successfuly");

            return result;
           
        } 
        public async Task<ProductBaseDTO> UpdateProductAsync(int id, UpdateProductDTO product, CancellationToken ct = default)
        {
            // 1. فتح الـ Transaction فوراً
            await _unitOfWork.BeginTransactionAsync();

            var existid = await _unitOfWork.Repositoey<Product>().GetByIdAsync(id,ct);
            if (existid == null)
                throw new KeyNotFoundException("Product ID Not Found");

            var existid2 = await _unitOfWork.Repositoey<Category>().GetByIdAsync(product.CategoryId,ct);
            if (existid2 == null)
                throw new KeyNotFoundException("Category ID Not Found");

            // خزن مسار الصورة القديمة
            string oldImagePath = existid.ImageUrl != null ?
                                  Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existid.ImageUrl.TrimStart('/'))
                                  : null;

            existid.Name = product.Name;
            existid.Description = product.Description;
            existid.CategoryId = product.CategoryId;
            existid.Price = product.Price;
            existid.Stock = product.Stock;

            string newImagePath = null;

            if (product.Image != null)
            {
                //if (!product.Image.ContentType.StartsWith("image/"))
                //    throw new FormatException("Invalid file type");

                //if (product.Image.Length > 2_000_000) // 2MB
                //    throw new ArgumentException("File too large");

                //if (!_allowedExtention.Contains(Path.GetExtension(product.Image.FileName).ToLower()))
                //    throw new ArgumentException("Only .png and .jpg images are allowed");

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(product.Image.FileName);
                newImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
                using (var stream = new FileStream(newImagePath, FileMode.Create))
                {
                    await product.Image.CopyToAsync(stream);
                }

                // خزن المسار الجديد جوه الـ Entity
                existid.ImageUrl = "/images/" + fileName;
            }

            try
            {
                _unitOfWork.Repositoey<Product>().Update(existid);
                await _unitOfWork.CompleteAsync();

                // 2. تعديل الشرط: احذف الصورة القديمة فقط لو تم رفع صورة جديدة بنجاح
                if (product.Image != null && oldImagePath != null && File.Exists(oldImagePath))
                    File.Delete(oldImagePath);

                // 3. عمل الـ Commit للعملية كلها
                await _unitOfWork.CommmetTransactionAsync();

                var result = mapper.Map<ProductBaseDTO>(existid);
                logger.LogInformation("Update Prodcut {Name} Successfuly. At {Time}.", product.Name, DateTime.Now);
                return result;
            }
            catch (Exception ex)
            {
                // 4. عمل Rollback للداتا بيز لو حصل خطأ
                await _unitOfWork.RollebackAsync();

                // امسح الصورة الجديدة لو حصل خطأ عشان السيرفر يفضل نضيف
                if (newImagePath != null && File.Exists(newImagePath))
                    File.Delete(newImagePath);
                logger.LogError("Update Prodcut {Name} Feiled. At {Time}.", product.Name, DateTime.Now);

                throw;
            }
        }
         
        public async Task<bool> CategoryExistsAsync(int categoryId, CancellationToken ct = default)
        {
            var categories = await _unitOfWork.Repositoey<Category>().GetAsync(c => c.Id == categoryId,ct);
            return categories.Any();
        }
         
        public async Task<List<ProductBaseDTO>> Search(string name, CancellationToken ct)
        {

            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Search Value is required");

            var products = await _unitOfWork.Repositoey<Product>().GetAsync(p => p.Name.Contains(name), ct);
            var result = mapper.Map<List<ProductBaseDTO>>(products);
            return result;
        }

        public async Task<ProductBaseDTO> UpdateStock(int id ,int stock, CancellationToken ct = default)
        {
            var product = await _unitOfWork.Repositoey<Product>().GetByIdAsync(id,ct);
            if (product == null)
                throw new ArgumentException("Product Not Found");

            if (stock < 0)
                throw new ArgumentException("Stock cannot be nagative");

            product.Stock = stock;
            await _unitOfWork.CompleteAsync();
            var result = mapper.Map<ProductBaseDTO>(product);
            logger.LogInformation("Update Stock {stock}. For ID {ID}. At {Time}.",stock, id, DateTime.Now);
            return result;

        }
         
    }
}
