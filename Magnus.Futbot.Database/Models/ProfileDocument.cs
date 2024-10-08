﻿using Magnus.Futbot.Common;
using Magnus.Futbot.Common.fcmodels;
using Magnus.Futbot.Common.Models.Database.Interfaces;
using Magnus.Futbot.Common.Models.Selenium.Trading;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Magnus.Futbot.Database.Models
{
    [BsonIgnoreExtraElements]
    public class ProfileDocument : IEntity
    {
        public ObjectId Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public ObjectId UserId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        public ProfileStatusType ProfilesStatus { get; set; }
        public int Coins { get; set; }

        public List<Auctioninfo> History { get; set; } = new List<Auctioninfo>();

        public TradePile TradePile { get; set; } = new TradePile();
        public bool AutoRelist { get; set; }
    }
}
