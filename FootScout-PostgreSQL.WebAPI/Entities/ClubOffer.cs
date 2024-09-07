using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FootScout_PostgreSQL.WebAPI.Entities
{
    public class ClubOffer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PlayerAdvertisementId { get; set; }

        [ForeignKey("PlayerAdvertisementId")]
        public virtual PlayerAdvertisement PlayerAdvertisement { get; set; }

        [Required]
        public int OfferStatusId { get; set; }

        [ForeignKey("OfferStatusId")]
        public virtual OfferStatus OfferStatus { get; set; }

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
        public double Salary { get; set; }

        [MaxLength(200)]
        public string AdditionalInformation { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreationDate { get; set; }

        [Required]
        public string ClubMemberId { get; set; }

        [ForeignKey("ClubMemberId")]
        public virtual User ClubMember { get; set; }
    }
}