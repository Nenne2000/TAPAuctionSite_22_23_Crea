using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TAP22_23.AlarmClock.Interface;
using TAP22_23.AuctionSite.Interface;
using static System.Collections.Specialized.BitVector32;

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
        public List<AuctionTable> AuctionSeller { get; set; }
        public List<AuctionTable> AuctionBidder { get; set; }
        public List<SessionTable> Sessions { get; set; }
        //--------------------------------------------------------//
        //modified
        public UserTable(string username, string password, int siteId)
        {
            Username = username;
            Password = password;
            SiteId = siteId;
            AuctionBidder = new List<AuctionTable>();
            AuctionSeller = new List<AuctionTable>();
            Sessions = new List<SessionTable>();
        }
    }
}
