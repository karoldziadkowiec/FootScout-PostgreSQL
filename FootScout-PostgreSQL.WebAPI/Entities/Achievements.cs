using System.ComponentModel.DataAnnotations;

namespace FootScout_PostgreSQL.WebAPI.Entities
{
    public class Achievements
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int NumberOfMatches { get; set; }

        [Required]
        public int Goals { get; set; }

        [Required]
        public int Assists { get; set; }

        [MaxLength(200)]
        public string AdditionalAchievements { get; set; }
    }
}