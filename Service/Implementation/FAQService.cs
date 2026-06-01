 
namespace E_Commerce_API.Service.Implementation
{
    public class FAQService : IFAQService
    {


        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger<FAQService> logger;

        public FAQService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<FAQService> logger)
        {
            _unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<FAQsDTO> AddFAQ(FAQsDTO fAQ)
        {
            var map = mapper.Map<FAQ>(fAQ);
           var result =  await _unitOfWork.Repositoey<FAQ>().AddAsync(map);
            await _unitOfWork.CompleteAsync();
            var endresult = mapper.Map<FAQsDTO>(result);
            logger.LogInformation("Add FAQ Quation {name} Successfuly. At {Time}", fAQ.Question, DateTime.Now);
            return endresult;
        }


        public async Task DeleteFAQ(int id, CancellationToken ct = default)
        {
            var getid = await _unitOfWork.Repositoey<FAQ>().GetByIdAsync(id,ct);
            if (getid == null)
                throw new KeyNotFoundException($" FAQ With Id {id} Not Found");

              _unitOfWork.Repositoey<FAQ>().Delete(getid);
            logger.LogInformation("Delete FAQ Quation ID {ID} Successfuly. At {Time}", id, DateTime.Now);
             await _unitOfWork.CompleteAsync();
 
         }

        public async Task <List<GetAllQuastionDTO>> GetAllQuastion(CancellationToken ct = default)
        {
             var result = await _unitOfWork.Repositoey<FAQ>().GetAll(ct);
             var map = mapper.Map<List<GetAllQuastionDTO>>(result);
            return map;

         }

        public async Task<AnswerDTO> GetAnswerById(int id, CancellationToken ct = default)
        {
            var getid = await _unitOfWork.Repositoey<FAQ>().GetByIdAsync(id,ct);
            if (getid == null)
                throw new KeyNotFoundException($"FAQ with id {id} not found");
            var result = mapper.Map<AnswerDTO>(getid);
             return result;
        }
         
        public async Task<FAQsDTO> UpdateFAQ(FAQtest fAQ, CancellationToken ct = default)
        {
            var getid = await _unitOfWork.Repositoey<FAQ>().GetByIdAsync(fAQ.Id,ct);
            if (getid == null)
                throw new KeyNotFoundException($"FAQ with id {fAQ.Id} not found");

            getid.Question = fAQ.Question;
            getid.Answer = fAQ.Answer;

               _unitOfWork.Repositoey<FAQ>().Update(getid);
             await _unitOfWork.CompleteAsync();
            var map = mapper.Map<FAQsDTO>(getid);
            logger.LogInformation("Update FAQ Successfuly. At {Time}.", DateTime.Now);
            return map;
        }
    }
}
