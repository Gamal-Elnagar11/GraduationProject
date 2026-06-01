 

namespace E_Commerce_API.Service.Implementation
{
    public class FeedbackService : IFeedbackService
    {

        private readonly Application _db;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMapper mapper;
        private readonly ILogger<FeedbackService> logger;
        public FeedbackService(Application db, IHttpContextAccessor contextAccessor, IMapper mapper, ILogger<FeedbackService> logger)
        {
            _db = db;
            _contextAccessor = contextAccessor;
            this.mapper = mapper;
            this.logger = logger;
        }
        public async Task AddFeedback(FeedDTO feedDTO)
        {
            var user = _contextAccessor.HttpContext?.User;

            if (user == null || !user.Identity.IsAuthenticated)
                throw new UnauthorizedAccessException("User not logged in");

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var usernameClaim = user.FindFirst(ClaimTypes.Name)?.Value;

            if (userIdClaim == null || usernameClaim == null)
                throw new UnauthorizedAccessException("User claims not found");


            if (feedDTO.Rating < 1 || feedDTO.Rating > 5)
                throw new ArgumentException(" Rating Must be betwen 1 and 5");
            var feedback = new Feedback
            {
                Comment = feedDTO.Comment,
                UserId = userIdClaim,
                UserName = usernameClaim,
                Rating = feedDTO.Rating,
                CreatedAt = DateTime.UtcNow
            };
             await _db.Feedbacks.AddAsync(feedback);
            await _db.SaveChangesAsync();
        }



        public async Task DeleteFeedback(int id)
        {
            var result = await _db.Feedbacks.FindAsync(id);
            if (result == null)
                throw new KeyNotFoundException($"this id {id} Not found");

              _db.Feedbacks.Remove(result);
             await  _db.SaveChangesAsync();
         }

        public async Task<List<FeedDTO>> GetAllFeedback()
        {
            var allfeed = await _db.Feedbacks.ToListAsync();
            var map = mapper.Map<List<FeedDTO>>(allfeed);
            return map;
        }
    }
}
