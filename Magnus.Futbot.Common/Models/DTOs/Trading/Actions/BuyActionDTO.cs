﻿namespace Magnus.Futbot.Common.Models.DTOs.Trading.Actions
{
    public class BuyActionDTO
    {
        public string Id { get; set; } = string.Empty;
        public TradeActionType Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Priority { get; set; }
        public string ProfileId { get; set; } = string.Empty;
        public BuyCardDTO? BuyCardDTO { get; set; }
    }
}
