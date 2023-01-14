using System.ComponentModel.DataAnnotations;

namespace Crea
{
    public class AuctionTable
    {
        [Key]
        public int AuctionId { get; set; }

        [Required]
        public string Description { get; set; }
        [Required]
        public DateTime EndsOn { get; set; }
        [Required]
        public double MaximumBidValue { get; set; }
        [Required]
        public double CurrentPrice { get; set; }

        //-----------------------------------------------//
        public UserTable? CreatedBy { get; set; }
        public int CreatedById { get; set; }
        public UserTable? CurrentWinner { get; set; }
        public int? CurrentWinnerId { get; set; }
        public SiteTable? Site { get; set; }
        public int SiteId { get; set; }
        //-----------------------------------------------//
        public AuctionTable(string description, DateTime endsOn, double currentPrice, int createdById, int siteId)
        {
            Description = description;
            EndsOn = endsOn;
            CreatedById = createdById;
            SiteId = siteId;
            MaximumBidValue = currentPrice;
            CurrentPrice = currentPrice;
        }
    }
}
