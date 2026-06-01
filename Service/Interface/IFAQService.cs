 

namespace E_Commerce_API.Service.Interface
{
    public interface IFAQService
    {
        public Task<List<GetAllQuastionDTO>> GetAllQuastion(CancellationToken ct = default);
        public Task<FAQsDTO> AddFAQ(FAQsDTO fAQ);
        public Task<FAQsDTO> UpdateFAQ(FAQtest fAQ, CancellationToken ct = default);
        public Task DeleteFAQ(int id, CancellationToken ct = default);
        public Task<AnswerDTO> GetAnswerById(int id, CancellationToken ct = default);

    }
}
