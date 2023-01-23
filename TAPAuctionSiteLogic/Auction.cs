using Microsoft.EntityFrameworkCore;

namespace Crea
{
    public class Auction : IAuction
    {
        public int Id { get; set; }

        public IUser Seller { get; set; }

        public string Description { get; set; }

        public DateTime EndsOn { get; set; }

        private readonly string _connectionString;
        private readonly Site _site;

        public Auction(int id, IUser seller,Site site, string description, DateTime endsOn, string connectionString)
        {
            Id = id;
            Seller = seller;
            Description = description;
            EndsOn = endsOn;
            _connectionString = connectionString;
            _site = site;
        }

        private void Exists()
        {
            using (var c = new DbContext(_connectionString))
            {
                var thisAuction = c.Auctions!.SingleOrDefault(a => a.AuctionId == Id && a.SiteId == _site.SiteId);
                if (thisAuction == null) throw new AuctionSiteInvalidOperationException("Auction Error: This auction doesn't exists");
            }
        }

        private void IncreaseTime(ISession session)
        {
            using (var c = new DbContext(_connectionString))
            {
                var s = c.Sessions.FirstOrDefault(s => s.SessionId == ((Session)session).Id);
                if (s == null) throw new AuctionSiteInvalidOperationException("Session not exist");
                s.ValidUntil = _site.Now().AddSeconds(_site.SessionExpirationInSeconds);
                c.SaveChanges();
            }
        }

        public bool Bid(ISession session, double offer)
        {
            Exists();

            if (session == null)
                throw new AuctionSiteArgumentNullException("Auction.bid Error: session cannot be null");
            if (EndsOn < _site.Now())
                throw new AuctionSiteInvalidOperationException("Auction.bid Error: auction closed");
            if (session.User.Username == Seller.Username)
                throw new AuctionSiteArgumentException("seller cannot make bids");
            if (offer < 0)
                throw new AuctionSiteArgumentOutOfRangeException("Auction.bid Error: Offer cannot be negative");
            if (session.ValidUntil < _site.Now()) throw new AuctionSiteArgumentException("Auction.bid Error: Session expired");

            using (var c = new DbContext(_connectionString))
            {
                var myUser = c.Users.FirstOrDefault(u => u.UserId == ((User)session.User).Id);
                if (myUser == null)
                    throw new AuctionSiteInvalidOperationException("Auction.bid Error: user deleted");

                var myAuction = c.Auctions.FirstOrDefault(a => a.AuctionId == Id);
                if (myAuction == null)
                    throw new AuctionSiteInvalidOperationException("Auction deleted");

                IncreaseTime(session);
                if (offer < 0) return false;
                if (offer < CurrentPrice()) return false;

                if (myAuction.CurrentWinnerId == myUser.UserId)
                {
                    if (offer < myAuction.MaximumBidValue + _site.MinimumBidIncrement)
                        return false;
                    myAuction.MaximumBidValue = offer;
                }
                else if (myAuction.CurrentWinnerId == null)
                {
                    myAuction.MaximumBidValue = offer;
                    myAuction.CurrentWinnerId = myUser.UserId;
                }
                else
                {
                    if (offer < CurrentPrice() + _site.MinimumBidIncrement)
                        return false;
                    if (myAuction.MaximumBidValue < offer)
                    {
                        if (offer < (myAuction.MaximumBidValue + _site.MinimumBidIncrement))
                            myAuction.CurrentPrice = offer;
                        else
                            myAuction.CurrentPrice = myAuction.MaximumBidValue + _site.MinimumBidIncrement;
                        myAuction.MaximumBidValue = offer;
                        myAuction.CurrentWinnerId = myUser.UserId;
                    }
                    else
                    {
                        if (myAuction.MaximumBidValue < (offer + _site.MinimumBidIncrement))
                            myAuction.CurrentPrice = myAuction.MaximumBidValue;
                        else
                            myAuction.CurrentPrice = Math.Min(myAuction.MaximumBidValue, offer + _site.MinimumBidIncrement);
                    }
                }
                c.SaveChanges();
            }
            return true;
        }

        public double CurrentPrice()
        {
            Exists();
            using (var c = new DbContext(_connectionString))
            {
                var price = c.Auctions.SingleOrDefault(a => a.AuctionId == Id && a.SiteId == _site.SiteId)?.CurrentPrice;
                if (price == null) return 0;
                return (double)price;
            }
        }

        public IUser? CurrentWinner()
        {
            using (var c = new DbContext(_connectionString))
            {
                var thisAuction = c.Auctions.Include(a => a.CurrentWinner).SingleOrDefault(a => a.SiteId == _site.SiteId && a.AuctionId == Id);
                if (thisAuction == null) throw new AuctionSiteInvalidOperationException("Auction Error: This auction doesn't exists");
                if (thisAuction.CurrentWinner == null)
                    return null;
                return new User(thisAuction.CurrentWinner.UserId,thisAuction.CurrentWinner.Username,thisAuction.CurrentWinner.Password,_connectionString,_site);
            }
        }

        public void Delete()
        {
            Exists();

            using (var c = new DbContext(_connectionString!))
            {
                var thisAuction = c.Auctions!.SingleOrDefault(a => a.AuctionId == Id && a.SiteId == _site.SiteId);

                if (thisAuction != null)
                {
                    c.Remove(thisAuction);
                    c.SaveChanges();
                }
            }
        }

        public override bool Equals(object? obj)
        {
            return obj is Auction auction &&
                   Id == auction.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
