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
}
