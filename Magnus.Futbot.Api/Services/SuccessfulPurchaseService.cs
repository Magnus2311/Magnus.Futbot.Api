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

        public async Task<IEnumerable<SuccessfulPurchaseResponse>> GetFilteredPurchasesAsync(string pidId, string? position = null, string? quality = null, string? league = null, string? club = null)
        {
            var purchases = await repository.GetFilteredPurchasesAsync(pidId, position, quality, league, club);
            return mapper.Map<IEnumerable<SuccessfulPurchaseResponse>>(purchases);
        }

        public async Task<IEnumerable<SuccessfulPurchaseResponse>> GetByFilterDescriptionAsync(string pidId, string filterDescription)
        {
            var purchases = await repository.GetByFilterDescriptionAsync(pidId, filterDescription);
            return mapper.Map<IEnumerable<SuccessfulPurchaseResponse>>(purchases);
        }

        public async Task<long> GetFilteredPurchasesCountAsync(string pidId)
        {
            return await repository.GetFilteredPurchasesCountAsync(pidId);
        }
    }
}
