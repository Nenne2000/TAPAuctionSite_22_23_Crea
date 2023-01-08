using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TAP22_23.AuctionSite.Interface;

namespace Crea
{
    [Index(nameof(Name), IsUnique = true, Name = "NameUnique")]
    public class SiteTable
    {
        [Key]
        public int SiteId { get; set; }

        [MaxLength(DomainConstraints.MaxSiteName)]
        [MinLength(DomainConstraints.MinSiteName)]
        public string Name { get; set; }

        [Required]//???
        [Range(DomainConstraints.MinTimeZone, DomainConstraints.MaxTimeZone)]
        public int Timezone { get; set; }

        [Range(0, int.MaxValue)]
        public int SessionExpirationInSeconds { get; set; }

        [Range(0, double.MaxValue)]
        public double MinimumBidIncrement { get; set; }
        //------------------------------------------------------------------//
        public List<UserTable> Users { get; set; }
        public List<AuctionTable> Auctions { get; set; }
        public List<SessionTable> Sessions { get; set; }
        //------------------------------------------------------------------//
        public SiteTable(string name, int timezone, int sessionExpirationInSeconds, double minimumBidIncrement)
        {
            Name = name;
            Timezone = timezone;
            SessionExpirationInSeconds = sessionExpirationInSeconds;
            MinimumBidIncrement = minimumBidIncrement;

            Users = new List<UserTable>();
            Auctions = new List<AuctionTable>();
            Sessions = new List<SessionTable>();
        }
    }
    /*
     [Index(nameof(Name), IsUnique = true, Name = "NameUnique")]
    public class SiteEntity {
        [Key]
        public int SiteId { get; set; }
        [MaxLength(DomainConstraints.MaxSiteName)]
        [MinLength(DomainConstraints.MinSiteName)]
        public string Name { get; set; }
        [Range(DomainConstraints.MinTimeZone, DomainConstraints.MaxTimeZone)] 
        public int Timezone { get; set; }
        [Range(0, int.MaxValue)]
        public int SessionExpirationInSeconds { get; set; }
        [Range(0, double.MaxValue)]
        public double MinimumBidIncrement { get; set; }
        
        //NAVIGATION
        public List<UserEntity> Users { get; set; }
        public List<AuctionEntity> Auctions { get; set; }
        public List<SessionEntity> Sessions { get; set; }


        public SiteEntity(string name, int timezone, int sessionExpirationInSeconds, double minimumBidIncrement) {
            Name = name;
            Timezone = timezone;
            SessionExpirationInSeconds = sessionExpirationInSeconds;
            MinimumBidIncrement = minimumBidIncrement;

            Users = new List<UserEntity>();
            Auctions = new List<AuctionEntity>();
            Sessions = new List<SessionEntity>();
        }
    }



    [Key]
        public int SiteId { get; set; }

        [NotMapped]
        private readonly IAlarmClock _alarmClock;

        [NotMapped]
        private readonly string _connectionString;

        [Required]
        [MinLength(DomainConstraints.MaxSiteName)]
        [MaxLength(DomainConstraints.MaxSiteName)]
        public string Name { get; set; }

        [Required]
        [Range(DomainConstraints.MinTimeZone, DomainConstraints.MaxTimeZone)]
        public int Timezone { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int SessionExpirationInSeconds { get; set; }

        [Required]
        [Range(double.Epsilon, double.PositiveInfinity)]
        public double MinimumBidIncrement { get; set; }

        public List<Auction> Auctions { get; set; } = new();
        public List<Session> Sessions { get; set; } = new();




        private Site() { }
        public Site(string name, int timeZone, int sessionExpirationInSeconds, double minimumBidIncrement, IAlarmClock alarmClock, string connectionString)
        {
            Name = name;
            Timezone = timeZone;
            SessionExpirationInSeconds = sessionExpirationInSeconds;
            MinimumBidIncrement = minimumBidIncrement;
            _alarmClock = alarmClock;
            _connectionString = connectionString;
        }
     */
}
