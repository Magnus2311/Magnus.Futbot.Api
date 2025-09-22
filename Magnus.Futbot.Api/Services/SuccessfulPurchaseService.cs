using Magnus.Futbot.Api.Models.Requests;
using Magnus.Futbot.Api.Models.Responses;
using Magnus.Futbot.Database.Models;
using Magnus.Futbot.Database.Repositories;
using AutoMapper;

namespace Magnus.Futbot.Api.Services
{
    public class SuccessfulPurchaseService(SuccessfulPurchaseRepository repository, IMapper mapper)
    {
        public async Task<SuccessfulPurchaseResponse> AddSuccessfulPurchaseAsync(AddSuccessfulPurchaseRequest request)
        {
            var successfulPurchase = mapper.Map<SuccessfulPurchase>(request);
            successfulPurchase.PurchaseDate = DateTime.Now;

            var addedPurchase = await repository.Add(successfulPurchase);
            return mapper.Map<SuccessfulPurchaseResponse>(addedPurchase);
        }

        public async Task<IEnumerable<SuccessfulPurchaseResponse>> GetByPidIdAsync(string pidId)
        {
            var purchases = await repository.GetByPidIdAsync(pidId);
            return mapper.Map<IEnumerable<SuccessfulPurchaseResponse>>(purchases);
        }

        public async Task<IEnumerable<SuccessfulPurchaseResponse>> GetByPidIdAndDateRangeAsync(string pidId, DateTime startDate, DateTime endDate)
        {
            var purchases = await repository.GetByPidIdAndDateRangeAsync(pidId, startDate, endDate);
            return mapper.Map<IEnumerable<SuccessfulPurchaseResponse>>(purchases);
        }

        public async Task<long> GetCountByPidIdAsync(string pidId)
        {
            return await repository.GetCountByPidIdAsync(pidId);
        }
    }
}
