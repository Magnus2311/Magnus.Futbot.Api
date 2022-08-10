using AutoMapper;
using Magnus.Futbot.Common.Interfaces;
using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Database.Models;
using Magnus.Futbot.Database.Repositories;

namespace Magnus.Futbot.Api.Services
{
    public class PlayersService : IPlayersService
    {
        private readonly PlayersRepository _playersRepository;
        private readonly IMapper _mapper;

        public PlayersService(PlayersRepository playersRepository,
            IMapper mapper)
        {
            _playersRepository = playersRepository;
            _mapper = mapper;
        }

        public async Task Add(IEnumerable<PlayerDTO> players)
        {
            var allIds = (await _playersRepository.GetAll()).Select(p => p.Id);
            var newPlayers = players.Where(p => !allIds.Contains(p.Id));

            await _playersRepository.AddPlayers(_mapper.Map<IEnumerable<PlayerDocument>>(players));
        }
    }
}
