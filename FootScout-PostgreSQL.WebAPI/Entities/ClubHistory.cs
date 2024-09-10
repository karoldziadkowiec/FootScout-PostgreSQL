using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FootScout_PostgreSQL.WebAPI.Entities
{
    public class ClubHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PlayerPositionId { get; set; }

        [Required]
        public virtual PlayerPosition PlayerPosition { get; set; }

        [Required]
        [MaxLength(30)]
        public string ClubName { get; set; }

        [Required]
        [MaxLength(30)]
        public string League { get; set; }

        [Required]
        [MaxLength(30)]
        public string Region { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }

        [Required]
        public int AchievementsId { get; set; }

        [ForeignKey("AchievementsId")]
        public virtual Achievements Achievements { get; set; }

        [Required]
        public string PlayerId { get; set; }

        [ForeignKey("PlayerId")]
        public virtual User Player { get; set; }
    }
}