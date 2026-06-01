
namespace E_Commerce_API.DTO.CartDTO
{
    public class ResponseCartDTO
    {
        public int Id { get; set; }

        public List<CartItemDTO> Items { get; set; } = new List<CartItemDTO>();

        public decimal TotalPrice => Items.Sum(item => item.TotalPrice);
    }
}
