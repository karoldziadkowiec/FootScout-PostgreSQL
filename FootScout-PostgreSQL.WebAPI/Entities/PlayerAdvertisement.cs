using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FootScout_PostgreSQL.WebAPI.Entities
{
    public class PlayerAdvertisement
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PlayerPositionId { get; set; }

        [ForeignKey("PlayerPositionId")]
        public virtual PlayerPosition PlayerPosition { get; set; }

        [Required]
        [MaxLength(30)]
        public string League { get; set; }

        [Required]
        [MaxLength(30)]
        public string Region { get; set; }

        [Required]
        public int Age { get; set; }

        [Required]
        public int Height { get; set; }

        [Required]
        public int PlayerFootId { get; set; }

        [ForeignKey("PlayerFootId")]
        public virtual PlayerFoot PlayerFoot { get; set; }

        [Required]
        public int SalaryRangeId { get; set; }

        [ForeignKey("SalaryRangeId")]
        public virtual SalaryRange SalaryRange { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreationDate { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }

        [Required]
        public string PlayerId { get; set; }

        [ForeignKey("PlayerId")]
        public virtual User Player { get; set; }
    }
}