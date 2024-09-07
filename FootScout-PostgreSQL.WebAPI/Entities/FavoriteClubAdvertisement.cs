using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FootScout_PostgreSQL.WebAPI.Entities
{
    public class FavoriteClubAdvertisement
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClubAdvertisementId { get; set; }

        [ForeignKey("ClubAdvertisementId")]
        public virtual ClubAdvertisement ClubAdvertisement { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}