using Magnus.Futbot.Api.Models.Requests;
using Magnus.Futbot.Api.Models.Responses;
using Magnus.Futbot.Database.Models;
using Magnus.Futbot.Database.Repositories;
using AutoMapper;

namespace Magnus.Futbot.Api.Services
{
    public class SuccessfulPurchaseService
    {
        private readonly SuccessfulPurchaseRepository _repository;
        private readonly IMapper _mapper;

        public SuccessfulPurchaseService(SuccessfulPurchaseRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<SuccessfulPurchaseResponse> AddSuccessfulPurchaseAsync(AddSuccessfulPurchaseRequest request)
        {
            var successfulPurchase = _mapper.Map<SuccessfulPurchase>(request);
            successfulPurchase.PurchaseDate = DateTime.Now;

            var addedPurchase = await _repository.Add(successfulPurchase);
            return _mapper.Map<SuccessfulPurchaseResponse>(addedPurchase);
        }

        public async Task<IEnumerable<SuccessfulPurchaseResponse>> GetByPidIdAsync(string pidId)
        {
            var purchases = await _repository.GetByPidIdAsync(pidId);
            return _mapper.Map<IEnumerable<SuccessfulPurchaseResponse>>(purchases);
        }

        public async Task<IEnumerable<SuccessfulPurchaseResponse>> GetByPidIdAndDateRangeAsync(string pidId, DateTime startDate, DateTime endDate)
        {
            var purchases = await _repository.GetByPidIdAndDateRangeAsync(pidId, startDate, endDate);
            return _mapper.Map<IEnumerable<SuccessfulPurchaseResponse>>(purchases);
        }

        public async Task<long> GetCountByPidIdAsync(string pidId)
        {
            return await _repository.GetCountByPidIdAsync(pidId);
        }

        public async Task<IEnumerable<SuccessfulPurchaseResponse>> GetByCardIdAsync(string cardId)
        {
            var purchases = await _repository.GetByCardIdAsync(cardId);
            return _mapper.Map<IEnumerable<SuccessfulPurchaseResponse>>(purchases);
        }
    }
}
