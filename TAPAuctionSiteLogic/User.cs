namespace Crea
{
    public class User : IUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        private readonly string _connectionString;
        private readonly Site _site;

        public User(int id, string username, string password, string connectionString, Site site)
        {
            Id = id;
            Username = username;
            Password = password;
            _connectionString = connectionString;
            _site = site;
        }

        private void Exists()
        {
            using (var c = new DbContext(_connectionString))
            {
                var thisUser = c.Users.SingleOrDefault(u => u.SiteId == Id && u.Username == Username);
                if (thisUser == null) throw new AuctionSiteInvalidOperationException("User Error: This user doesn't exists");
            }
        }

        public void Delete()
        {
            Exists();
            var OpenAuction = _site.ToyGetAuctions(true).FirstOrDefault(a => a.Seller.Username == Username || a.CurrentWinner()!.Username == Username);
            if (OpenAuction != null) throw new AuctionSiteInvalidOperationException("User.Delete Error: you cannot delete user with open auction");
            using (var c = new DbContext(_connectionString))
            {
                var user = c.Users.FirstOrDefault(u => u.UserId == Id);
                if (user == null) throw new AuctionSiteInvalidOperationException("User.Delete Error: no user");

                var auctions = _site.ToyGetAuctions(false).Where(s => s.Seller.Equals(this));
                var sessions = _site.ToyGetSessions().Where(s => s.User.Username == Username).ToList();

                foreach (var a in auctions)
                    a.Delete();

                foreach (var s in sessions)
                    s.Logout();

                c.Users.Remove(user);

                c.SaveChanges();
            }
        }

        public IEnumerable<IAuction> WonAuctions()
        {
            Exists();

            var wonAuctionsList = new List<AuctionTable>();
            using (var c = new DbContext(_connectionString))
            {
                wonAuctionsList = c.Auctions.Where(a => a.EndsOn < _site.Now() && a.CurrentWinnerId == Id).ToList();
                foreach (var a in wonAuctionsList)
                {
                    var user = c.Users.FirstOrDefault(u => u.UserId == a.CurrentWinnerId);
                    if (user == null) throw new AuctionSiteInvalidOperationException("User.WonAuction Error: no user");
                    yield return new Auction(a.AuctionId, new User(user.UserId,user.Username, user.Password,_connectionString,_site),_site, a.Description, a.EndsOn, _connectionString);
                }
            }
        }

        public override bool Equals(object? obj)
        {
            return obj is User user &&
                   Id == user.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
