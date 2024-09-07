using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootScout_PostgreSQL.WebAPI.Entities
{
    public class FavoritePlayerAdvertisement
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PlayerAdvertisementId { get; set; }

        [ForeignKey("PlayerAdvertisementId")]
        public virtual PlayerAdvertisement PlayerAdvertisement { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}