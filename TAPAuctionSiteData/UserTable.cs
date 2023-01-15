using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TAP22_23.AuctionSite.Interface;

namespace Crea
{
    [Index(nameof(SiteId), nameof(Username), IsUnique = true, Name = "UsernameUnique")]
    public class UserTable
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MinLength(DomainConstraints.MinUserName)]
        [MaxLength(DomainConstraints.MaxUserName)]
        public string Username { get; set; }

        [Required]
        [MinLength(DomainConstraints.MinUserPassword)]
        public string Password { get; set; }

        //--------------------------------------------------------//
        public SiteTable? SiteEntity { get; set; }
        public int SiteId { get; set; }
        public List<AuctionTable> AuctionSeller { get; set; } = new();
        public List<AuctionTable> AuctionBidder { get; set; } = new();
        public List<SessionTable> Sessions { get; set; } = new();
        //--------------------------------------------------------//
        public UserTable(string username, string password, int siteId)
        {
            Username = username;
            Password = password;
            SiteId = siteId;
        }
    }
}
