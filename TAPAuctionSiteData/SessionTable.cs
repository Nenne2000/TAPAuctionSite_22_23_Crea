using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAP22_23.AlarmClock.Interface;
using TAP22_23.AuctionSite.Interface;

namespace Crea
{
    public class SessionTable
    {
        [Key]
        public string SessionId { get; set; }
        public DateTime ValidUntil { get; set; }

        //-----------------------------------------//
        public UserTable? Owner { get; set; }
        public int UserId { get; set; }

        public SiteTable? Site { get; set; }
        public int SiteId { get; set; }
        //------------------------------------------//
        public SessionTable(string sessionId, DateTime validUntil, int userId, int siteId)
        {
            SessionId = sessionId;
            ValidUntil = validUntil;
            UserId = userId;
            SiteId = siteId;
        }
    }
    /*
    [Key]
    public string Id { get; set; }

    public User? Owner { get; set; }
    public int UserId { get; set; }

    public Site? Site { get; set; }
    public int SiteId { get; set; }

    public DateTime DbValidUntil { get; set; }

    [NotMapped]
    public IUser User { get; set; }

    [NotMapped]
    private readonly string _connectionString;

    [NotMapped]
    private readonly IAlarmClock _alarmClock;


    [NotMapped]
    public DateTime ValidUntil
    {
        get
        {
            using (var c = new DbContext(_connectionString))
            {
                var thisSession = c.Sessions!.SingleOrDefault(s => s.Id == Id);
                if (thisSession == null) return DbValidUntil;
                return thisSession.DbValidUntil;
            }
        }
        set
        {
            DbValidUntil = value;
        }
    }

    private Session() { }

    public Session(int siteId, int userId, User user, DateTime validUntil, string connectionString, IAlarmClock alarmClock)
    {
        Id = userId.ToString();
        _connectionString = connectionString;
        SiteId = siteId;
        UserId = userId;
        ValidUntil = validUntil;
        User = user;
        _alarmClock = alarmClock;
    }*/
}
