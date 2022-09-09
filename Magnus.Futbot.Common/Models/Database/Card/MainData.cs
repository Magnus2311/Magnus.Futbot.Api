﻿namespace Magnus.Futbot.Common.Models.Database.Card
{
    public class MainData
    {
        public int SkillRating { get; internal set; }
        public int WeakFootRating { get; internal set; }
        public int IntlReputation { get; internal set; }
        public string Foot { get; internal set; } = string.Empty;
        public string Height { get; internal set; } = string.Empty;
        public string Weight { get; internal set; } = string.Empty;
        public string DefensiveWorkRate { get; internal set; } = string.Empty;
        public string AttackingWorkRate { get; internal set; } = string.Empty;
        public string Origin { get; internal set; } = string.Empty;
    }
}