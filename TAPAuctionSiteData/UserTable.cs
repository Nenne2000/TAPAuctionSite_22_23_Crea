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
        public string Password { get; set; }
        //--------------------------------------------------------//
        public SiteTable? SiteEntity { get; set; }
        public int SiteId { get; set; }
        public SessionTable? Session { get; set; }
        public string? SessionId { get; set; } = null;
        public List<AuctionTable> AuctionSeller { get; set; }
        public List<AuctionTable> AuctionBidder { get; set; }
        //--------------------------------------------------------//
        //modified
        public UserTable(string username, string password, int siteId, string sessionId)
        {
            Username = username;
            Password = password;
            SiteId = siteId;
            SessionId = sessionId;
            AuctionBidder = new List<AuctionTable>();
            AuctionSeller = new List<AuctionTable>();
        }
    }

    /*
    [Index(nameof(SiteId), nameof(Username), IsUnique = true, Name = "UsernameUnique")]
    public class User : IUser
    {
        [Key]
        public int UserId { get; set; }

        public Site? Site { get; set; }
        public int SiteId { get; }

        public Session? Session { get; set; }
        public string? SessionId { get; set; } = null;

        [Required]
        [MinLength(DomainConstraints.MinUserName)]
        [MaxLength(DomainConstraints.MaxUserName)]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public List<Auction> CurrentlyWinning { get; set; } = new();
        public List<Auction> Selling { get; set; } = new();

        [NotMapped]
        private readonly string _connectionString;

        [NotMapped]
        private readonly IAlarmClock _alarmClock;

        private User() { }

        public User(int siteId, string username, string password, string connectionString, IAlarmClock alarmClock)
        {
            Username = username;
            Password = password;
            SiteId = siteId;
            _connectionString = connectionString;
            _alarmClock = alarmClock;
        }





        [Index(nameof(Username), nameof(SiteId), IsUnique = true, Name = "UsernameUnique")]
        public class UserEntity
        {
            [Key]
            public int UserId { get; set; }
            [MinLength(DomainConstraints.MinUserName)]
            [MaxLength(DomainConstraints.MaxUserName)]
            [Required]
            public string Username { get; set; }
            public string Password { get; set; } //HASH DELLA PASSWORD

            //NAVIGATION

            public SiteEntity? SiteEntity { get; set; }
            public int SiteId { get; set; }
            public List<AuctionEntity> AuctionSeller { get; set; }
            public List<AuctionEntity> AuctionBidder { get; set; }

            public List<SessionEntity> Sessions { get; set; }

            public UserEntity(string username, string password, int siteId)
            {
                Username = username;
                Password = password;
                SiteId = siteId;
                AuctionBidder = new List<AuctionEntity>();
                AuctionSeller = new List<AuctionEntity>();
                Sessions = new List<SessionEntity>();
            }
        }
    */
    }
