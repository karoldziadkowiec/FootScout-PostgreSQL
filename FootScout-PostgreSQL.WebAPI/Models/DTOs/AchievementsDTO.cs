namespace FootScout_PostgreSQL.WebAPI.Models.DTOs
{
    public class AchievementsDTO
    {
        public int NumberOfMatches { get; set; }
        public int Goals { get; set; }
        public int Assists { get; set; }
        public string AdditionalAchievements { get; set; }
    }
}