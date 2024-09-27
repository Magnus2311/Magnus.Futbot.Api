using Magnus.Futbot.Common.Models.Database.Interfaces;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace Magnus.Futbot.Common.Models.Database.Card
{
    public class Card : IEntity
    {
        public int EAId { get; set; }
        public int Rank { get; set; }
        public int OverallRating { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CommonName { get; set; }
        public string Name => FirstName + " " + LastName;
        public string Birthdate { get; set; }
        public int Height { get; set; }
        public int SkillMoves { get; set; }
        public int WeakFootAbility { get; set; }
        public int AttackingWorkRate { get; set; }
        public int DefensiveWorkRate { get; set; }
        public int PreferredFoot { get; set; }
        public string LeagueName { get; set; }
        public int Weight { get; set; }
        public string AvatarUrl { get; set; }
        public string ShieldUrl { get; set; }

        [JsonPropertyName("alternatePositions")]
        public List<AlternatePosition> AlternatePositions { get; set; }

        [JsonPropertyName("playStyle")]
        public List<PlayStyle> PlayStyles { get; set; }

        public PlayStylePlus PlayStylePlus { get; set; }

        public Gender Gender { get; set; }

        public Nationality Nationality { get; set; }

        public Team Team { get; set; }

        public Position Position { get; set; }

        public Stats Stats { get; set; }
        public ObjectId Id { get; set; }
        public string CardId
        {
            get
            {
                return Id.ToString();
            }
            set
            {
                Id = new ObjectId(value);
            }
        }
        public bool IsDeleted { get; set; }
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public ObjectId UserId { get; set; }
        public int AssetId { get; internal set; }
        public int ClubId { get; internal set; }
        public int LeagueId { get; internal set; }
        public int NationId { get; internal set; }

        
    }

    public class AlternatePosition
    {
        public string Id { get; set; }
        public string Label { get; set; }

        [JsonPropertyName("shortLabel")]
        public string ShortLabel { get; set; }
    }

    public class PlayStyle
    {
        public string Id { get; set; }
        public string Label { get; set; }

        [JsonPropertyName("imageUrl")]
        public string ImageUrl { get; set; }
    }

    public class PlayStylePlus
    {
        // Define properties for PlayStylePlus
    }

    public class Gender
    {
        public int Id { get; set; }
        public string Label { get; set; }
    }

    public class Nationality
    {
        public int Id { get; set; }
        public string Label { get; set; }

        [JsonPropertyName("imageUrl")]
        public string ImageUrl { get; set; }
    }

    public class Team
    {
        public int Id { get; set; }
        public string Label { get; set; }

        [JsonPropertyName("imageUrl")]
        public string ImageUrl { get; set; }

        [JsonPropertyName("isPopular")]
        public bool IsPopular { get; set; }
    }

    public class Position
    {
        public string Id { get; set; }

        [JsonPropertyName("shortLabel")]
        public string ShortLabel { get; set; }

        public string Label { get; set; }

        public Positiontype PositionType { get; set; }
    }

    public class Positiontype
    {
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class Stats
    {
        public Stat Acceleration { get; set; }
        public Stat Aggression { get; set; }
        public Stat Agility { get; set; }
        public Stat Balance { get; set; }
        public Stat BallControl { get; set; }
        public Stat Composure { get; set; }
        public Stat Crossing { get; set; }
        public Stat Curve { get; set; }
        public Stat Def { get; set; }
        public Stat DefensiveAwareness { get; set; }
        public Stat Dri { get; set; }
        public Stat Dribbling { get; set; }
        public Stat Finishing { get; set; }
        public Stat FreeKickAccuracy { get; set; }
        public Stat GkDiving { get; set; }
        public Stat GkHandling { get; set; }
        public Stat GkKicking { get; set; }
        public Stat GkPositioning { get; set; }
        public Stat GkReflexes { get; set; }
        public Stat HeadingAccuracy { get; set; }
        public Stat Interceptions { get; set; }
        public Stat Jumping { get; set; }
        public Stat LongPassing { get; set; }
        public Stat LongShots { get; set; }
        public Stat Pac { get; set; }
        public Stat Pas { get; set; }
        public Stat Penalties { get; set; }
        public Stat Phy { get; set; }
        public Stat Positioning { get; set; }
        public Stat Reactions { get; set; }
        public Stat Sho { get; set; }
        public Stat ShortPassing { get; set; }
        public Stat ShotPower { get; set; }
        public Stat SlidingTackle { get; set; }
        public Stat SprintSpeed { get; set; }
        public Stat Stamina { get; set; }
        public Stat StandingTackle { get; set; }
        public Stat Strength { get; set; }
        public Stat Vision { get; set; }
        public Stat Volleys { get; set; }
    }

    public class Stat
    {
        public int Value { get; set; }
        public int Diff { get; set; }
    }
}
