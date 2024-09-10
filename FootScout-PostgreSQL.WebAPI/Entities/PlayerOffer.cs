using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FootScout_PostgreSQL.WebAPI.Entities
{
    public class PlayerOffer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClubAdvertisementId { get; set; }

        [ForeignKey("ClubAdvertisementId")]
        public virtual ClubAdvertisement ClubAdvertisement { get; set; }

        [Required]
        public int OfferStatusId { get; set; }

        [Required]
        public virtual OfferStatus OfferStatus { get; set; }

        [Required]
        public int PlayerPositionId { get; set; }

        [Required]
        public virtual PlayerPosition PlayerPosition { get; set; }

        [Required]
        public int Age { get; set; }

        [Required]
        public int Height { get; set; }

        [Required]
        public int PlayerFootId { get; set; }

        [Required]
        public virtual PlayerFoot PlayerFoot { get; set; }

        [Required]
        public double Salary { get; set; }

        [MaxLength(200)]
        public string AdditionalInformation { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreationDate { get; set; }

        [Required]
        public string PlayerId { get; set; }

        [ForeignKey("PlayerId")]
        public virtual User Player { get; set; }
    }
}