using Microsoft.Data.SqlClient;

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
                    if (thisSession == null) throw new AuctionSiteArgumentException("Session Error: Session Deleted");
                    return thisSession.ValidUntil;
                }
            }
        }


        public IUser User { get; set; }

        private readonly string _connectionString;
        private readonly Site _site;

        public Session(string id, IUser user, string connectionString, Site site)
        {
            Id = id;
            User = user;
            _connectionString = connectionString;
            _site = site;
        }
        private void Exists()
        {
            using (var c = new DbContext(_connectionString))
            {
                var thisSession = c.Sessions!.SingleOrDefault(s => s.SessionId == Id);
                if (thisSession == null) throw new AuctionSiteInvalidOperationException("Session Error: This session has been deleted");
            }
        }

        public IAuction CreateAuction(string description, DateTime endsOn, double startingPrice)
        {
            Exists();

            if (_site.Now() > ValidUntil) throw new AuctionSiteInvalidOperationException("Session.CreateAuction Error: This session has expired");

            if (description == null)
                throw new AuctionSiteArgumentNullException("Session.CreateAuction Error: The description cannot be empty");
            if (description == "") throw new AuctionSiteArgumentException("Session.CreateAuction Error: The description cannot be empty");
            if (startingPrice < 0)
                throw new AuctionSiteArgumentOutOfRangeException(nameof(startingPrice), startingPrice, "Session.CreateAuction Error: The starting price cannot be negative");
            if (endsOn < _site.Now())
                throw new AuctionSiteUnavailableTimeMachineException("Session.CreateAuction Error: endsOn cannot be less than the current time");

            using (var c = new DbContext(_connectionString))
            {
                var user = c.Users!.SingleOrDefault(u => u.SiteId == _site.SiteId && u.Username == User.Username);
                if (user == null) throw new AuctionSiteUnavailableDbException("Session.CreateAuction Error: User not found");
                var newAuction = new AuctionTable(description,endsOn,startingPrice,((User)User).Id,_site.SiteId);
                try
                {
                    c.Auctions.Add(newAuction);
                    c.SaveChanges();
                }
                catch (SqlException e)
                {
                    throw new AuctionSiteUnavailableDbException("DB Error", e);
                }

                var thisSession = c.Sessions!.SingleOrDefault(s => s.SessionId == Id);
                if (thisSession == null)
                    throw new AuctionSiteInvalidOperationException("Session.CreateAuction Error: Session deleted");
                thisSession.ValidUntil = _site.Now().AddSeconds(_site.SessionExpirationInSeconds);
                c.SaveChanges();

                return new Auction(newAuction.AuctionId,User,_site,description,endsOn,_connectionString);
            }
        }
        public void Logout()
        {
            Exists();
            using (var c = new DbContext(_connectionString))
            {
                try
                {
                    var mySession = c.Sessions.FirstOrDefault(s => s.SessionId == Id);
                    if (mySession == null) throw new AuctionSiteInvalidOperationException("Session.Logout Error: Session deleted or not exist");
                    c.Sessions.Remove(mySession);
                    c.SaveChanges();
                }
                catch (SqlException e)
                {
                    throw new AuctionSiteUnavailableDbException("DB Error", e);
                }
            }
        }

        public override bool Equals(object? obj)
        {
            return obj is Session session &&
                   Id == session.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
