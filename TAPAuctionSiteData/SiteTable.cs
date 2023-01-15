using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
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

        [Required]
        [Range(DomainConstraints.MinTimeZone, DomainConstraints.MaxTimeZone)]
        public int Timezone { get; set; }

        [Range(0, int.MaxValue)]
        public int SessionExpirationInSeconds { get; set; }

        [Range(0, double.MaxValue)]
        public double MinimumBidIncrement { get; set; }
        //------------------------------------------------------------------//
        public List<UserTable> Users { get; set; } = new();
        public List<AuctionTable> Auctions { get; set; } = new();
        public List<SessionTable> Sessions { get; set; } = new();
        //------------------------------------------------------------------//
        public SiteTable(string name, int timezone, int sessionExpirationInSeconds, double minimumBidIncrement)
        {
            Name = name;
            Timezone = timezone;
            SessionExpirationInSeconds = sessionExpirationInSeconds;
            MinimumBidIncrement = minimumBidIncrement;
        }
    }
}
