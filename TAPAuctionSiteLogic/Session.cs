using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAP22_23.AuctionSite.Interface;

namespace Crea
{
    public class Session : ISession
    {
        public string Id { get; set; }

        public DateTime ValidUntil
        {
            get
            {
                using (var c = new DbContext(_connectionString))
                {
                    var thisSession = c.Sessions!.SingleOrDefault(s => s.SessionId == Id);
                    if (thisSession == null) throw new AuctionSiteInvalidOperationException("Session Error: Session Deleted");
                    return thisSession.ValidUntil;
                }
            }
        }


        public IUser User { get; set; }

        private readonly string _connectionString;
        private readonly Site _site;

        public Session(string id, DateTime validUntil, IUser user, string connectionString, Site site)
        {
            Id = id;
            User = user;
            _connectionString = connectionString;
            _site = site;
        }

        public IAuction CreateAuction(string description, DateTime endsOn, double startingPrice)
        {
            throw new NotImplementedException();
        }

        public void Logout()
        {
            throw new NotImplementedException();
        }
    }
}
