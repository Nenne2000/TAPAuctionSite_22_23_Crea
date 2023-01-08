using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


/*
        [NotMapped]
        private readonly string _connectionString;

        [NotMapped]
        private readonly IAlarmClock _alarmClock;

        [Key]
        public int AuctionId { get; set; }

        [NotMapped]
        public int Id { get; }

        public User? CreatedBy { get; set; } 

        public int SellingUserId { get; set; }

        [NotMapped]
        public IUser Seller { get; set; }

        public User? CurWinner { get; set; }
        public int? CurWinnerId { get; set; }

        public Site? Site { get; set; }
        public int SiteId { get; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime EndsOn { get; set; }

        [Required]
        public double StartPrice { get; set; }

        [Required]
        public double CurPrice { get; set; }

        public double MaximumAmount { get; set; } = 0;

        [NotMapped]
        private ISession? LastBidderSession { get; set; }
        [NotMapped]
        private ISession? CurBidderSession { get; set; }

        [NotMapped]
        public int NumberOfBids { get; set; }

        private Auction() { }

        public Auction(int id, int sellingUserId, IUser seller, int siteId, string description, DateTime endsOn, double startPrice, double curPrice, string connectionString, IAlarmClock alarmClock)
        {
            Id = id;
            SellingUserId = sellingUserId;
            Seller = seller;
            SiteId = siteId;
            Description = description;
            EndsOn = endsOn;
            _connectionString = connectionString;
            StartPrice = startPrice;
            CurPrice = curPrice;
            _alarmClock = alarmClock;
            NumberOfBids = 0;
        }

 */