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
}
