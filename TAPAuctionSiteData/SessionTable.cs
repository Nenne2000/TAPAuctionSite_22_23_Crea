using System.ComponentModel.DataAnnotations;

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
