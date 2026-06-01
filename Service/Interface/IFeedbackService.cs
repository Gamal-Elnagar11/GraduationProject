 
namespace E_Commerce_API.Service.Interface
{
    public interface IFeedbackService
    {
        public Task AddFeedback(FeedDTO feedDTO);
        public Task<List<FeedDTO>> GetAllFeedback();
        public Task DeleteFeedback(int id);
    }
}
