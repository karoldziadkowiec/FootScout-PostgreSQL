using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FootScout_PostgreSQL.WebAPI.Entities
{
    public class ClubAdvertisement
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PlayerPositionId { get; set; }

        [ForeignKey("PlayerPositionId")]
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
        public string ClubMemberId { get; set; }

        [ForeignKey("ClubMemberId")]
        public virtual User ClubMember { get; set; }
    }
}