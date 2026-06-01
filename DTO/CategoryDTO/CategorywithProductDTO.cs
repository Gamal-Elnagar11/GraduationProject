 
namespace E_Commerce_API.DTO.CategoryDTO
{
    public class CategorywithProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ProductBaseDTO> Products { get; set; }
    }
}
