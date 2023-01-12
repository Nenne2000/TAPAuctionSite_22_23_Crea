using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TAP22_23.AlarmClock.Interface;
using TAP22_23.AuctionSite.Interface;
using static System.Collections.Specialized.BitVector32;

namespace Crea
{
    public class Site : ISite
    {
        public int SiteId { get; set; }

        public string Name { get; set; }

        public int Timezone { get; set; }

        public int SessionExpirationInSeconds { get; set; }

        public double MinimumBidIncrement { get; set; }

        private readonly string _connectionString;
        private readonly IAlarmClock _alarmClock;
        private IAlarm Alarm;

        public Site(int siteId, string name, int timezone, int sessionExpirationInSeconds, double minimumBidIncrement, string connectionString, IAlarmClock alarmClock)
        {
            SiteId = siteId;
            Name = name;
            Timezone = timezone;
            SessionExpirationInSeconds = sessionExpirationInSeconds;
            MinimumBidIncrement = minimumBidIncrement;
            _connectionString = connectionString;
            _alarmClock = alarmClock;
            Alarm = _alarmClock.InstantiateAlarm(5 * 60 * 1000);
            Alarm.RingingEvent += CleanSession;
        }

        public void CleanSession()
        {
            using (var c = new DbContext(_connectionString))
            {
                var sessionsToClean =
                    c.Sessions.Where(s => s.SiteId == SiteId && s.ValidUntil <= _alarmClock.Now);
                c.Sessions.RemoveRange(sessionsToClean);
                c.SaveChanges();
            }
            Alarm = _alarmClock.InstantiateAlarm(5 * 60 * 1000);
        }
        private void Exists()
        {
            using (var c = new DbContext(_connectionString))
            {
                var thisSite = c.Sites.SingleOrDefault(s => s.SiteId == SiteId);
                if (thisSite == null) throw new AuctionSiteInvalidOperationException("Site Error: This site doesn't exists");
            }
        }
        public IEnumerable<IUser> ToyGetUsers()
        {
            IEnumerable<IUser> ToyGetUsers_aux(List<UserTable> users)
            {
                foreach (var user in users)
                {
                    yield return new User(user.UserId,user.Username,user.Password, _connectionString, this);
                }
            }

            Exists();
            using (var c = new DbContext(_connectionString))
            {
                var users = c.Users.Where(s => s.SiteId == SiteId).ToList();
                return ToyGetUsers_aux(users);
            }
        }

        public IEnumerable<ISession> ToyGetSessions()
        {
            IEnumerable<ISession> ToyGetSessions_aux(List<SessionTable> sessions)
            {
                foreach (var session in sessions)
                {
                    if (session.Owner == null) throw new AuctionSiteArgumentNullException("Session owner cannot be null");
                    var user_aux = new User(session.Owner.UserId,session.Owner.Username,session.Owner.Password, _connectionString, this);
                    yield return new Session(session.SessionId,session.ValidUntil, user_aux , _connectionString, this);
                }
            }
            Exists();
            using (var c = new DbContext(_connectionString))
            {
                var sessions = c.Sessions.Where(s => s.SiteId == SiteId).Include(s => s.Owner).ToList();
                return ToyGetSessions_aux(sessions);
            }
        }

        public IEnumerable<IAuction> ToyGetAuctions(bool onlyNotEnded)
        {
            IEnumerable<IAuction> ToyGetAuction_aux(List<AuctionTable> auctions)
            {
                foreach (var auction in auctions)
                {
                    var user = auction.CreatedBy;
                    if (user == null) throw new AuctionSiteArgumentNullException("Auction owner cannot be null");
                    var newUser = new User(user.UserId,user.Username,user.Password, _connectionString, this);
                    yield return new Auction(auction.AuctionId, newUser,this, auction.Description, auction.EndsOn, _connectionString);
                }
            }
            Exists();
            using (var c = new DbContext(_connectionString))
            {
                List<AuctionTable> auctions;
                if (onlyNotEnded)
                    auctions = c.Auctions.Include(a => a.CreatedBy).Where(a => a.SiteId == SiteId && a.EndsOn > Now()).ToList();
                else
                    auctions = c.Auctions.Include(a => a.CreatedBy).Where(a => a.SiteId == SiteId).ToList();
                return ToyGetAuction_aux(auctions);
            }
        }

        public ISession? Login(string username, string password)
        {
            throw new NotImplementedException();
            /*
            Exists();
            if (username == null || password == null)
                throw new AuctionSiteArgumentNullException("user Error: Username or password cannot be null");
            if (username.Length < DomainConstraints.MinUserName)
                throw new AuctionSiteArgumentException("Login.CreateUser Error: username too short", nameof(username));
            if (username.Length > DomainConstraints.MaxUserName)
                throw new AuctionSiteArgumentException("Login.CreateUser Error: username too long", nameof(username));
            if (password.Length < DomainConstraints.MinUserPassword)
                throw new AuctionSiteArgumentException("Login.CreateUser Error: password too short ", nameof(password));

            using (var c = new DbContext(_connectionString))
            {
                var myUser = c.Users!.Include(u => u.Session).SingleOrDefault(u => u.Username == username && u.SiteId == SiteId && u.Password == Hash.GenerateHash(password));
                if (myUser == null) return null;
                if (myUser.SessionId == null)
                {
                    var newSession = new Session(SiteId, myUser.UserId,
                        new User(SiteId, username, Hash.GenerateHash(password), _connectionString, _alarmClock), _alarmClock.Now.AddSeconds(SessionExpirationInSeconds), _connectionString, _alarmClock)
                    { Id = myUser.UserId.ToString() };
                    myUser.SessionId = newSession.Id;
                    c.Sessions!.Add(newSession);
                    c.SaveChanges();
                    return newSession;
                }
                else
                {
                    var session = myUser.Session;
                    session!.ValidUntil = _alarmClock.Now.AddSeconds(SessionExpirationInSeconds);
                    c.Update(session);
                    c.SaveChanges();
                    var newSession = new Session(SiteId, myUser.UserId,
                        new User(SiteId, username, Hash.GenerateHash(password), _connectionString, _alarmClock),
                        session.DbValidUntil, _connectionString, _alarmClock)
                    { Id = session.Id };
                    return newSession;
                }
            }*/
        }

        public void CreateUser(string username, string password)
        {
            Exists();
            if (username == null || password == null)
                throw new AuctionSiteArgumentNullException("user Error: Username or password cannot be null");
            if (username.Length < DomainConstraints.MinUserName || username.Length > DomainConstraints.MaxUserName)
                throw new AuctionSiteArgumentException("user Error: Username Lenght too short or too long");
            if (password.Length < DomainConstraints.MinUserPassword)
                throw new AuctionSiteArgumentException("user Error: password too short");

            var newUser = new UserTable(username, password, SiteId);

            using(var c = new DbContext(_connectionString))
            {
                UserTable? alreadyExistingUser = c.Users.SingleOrDefault(u => u.Username == username);
                if (alreadyExistingUser != null)
                    throw new AuctionSiteNameAlreadyInUseException("User.CreateUser Error: user already exists");
                c.Users.Add(newUser);
                c.SaveChanges();
            }
        }

        public void Delete()
        {
            //Exists();
            using (var c = new DbContext(_connectionString))
            {
                var sessions = ToyGetSessions();
                foreach (var s in sessions)
                    s.Logout();
                var users = ToyGetUsers();
                foreach (var u in users)
                    u.Delete();
                var thisSite = c.Sites.FirstOrDefault(s => s.SiteId == SiteId);
                if (thisSite == null) throw new AuctionSiteInvalidOperationException("Site Error: This site doesn't exists");
                c.Sites.Remove(thisSite);

                c.SaveChanges();
            }
        }

        public DateTime Now()
        {
            Exists();
            return _alarmClock.Now;
        }


        /*
        public Site(int id, string name, int timezone, int sessionExpirationInSeconds, double minimumBidIncrement, string connectionString, IAlarmClock alarmClock)
        {
            this.Id = id;
            Name = name;
            Timezone = timezone;
            SessionExpirationInSeconds = sessionExpirationInSeconds;
            MinimumBidIncrement = minimumBidIncrement;
            _connectionString = connectionString;
            _alarmClock = alarmClock;
            alarm = _alarmClock.InstantiateAlarm(300000);
            alarm.RingingEvent += CleanSession;
        }

        private void Exists()
        {
            using (var c = new DbContext(_connectionString))
            {
                var thisSite = c.Sites.SingleOrDefault(s => s.Name == Name);
                if (thisSite == null) throw new AuctionSiteInvalidOperationException("Site Error: This site doesn't exists");
            }
        }
        private UserTable UserValidator(string username, string password)
        {
            if (string.IsNullOrEmpty(password)) throw new AuctionSiteArgumentNullException("password cannot be null");
            

            var newUser = new UserTable(username, password, Id);
            var vc = new ValidationContext(newUser);
            try
            {
                Validator.ValidateObject(newUser, vc, true);
            }
            catch (ValidationException e)
            {
                if (e.ValidationAttribute!.GetType() == typeof(MinLengthAttribute))
                    throw new AuctionSiteArgumentException("Site.UserValidation Error: value too short", $"{e.ValidationResult.MemberNames.First()}", e);
                if (e.ValidationAttribute.GetType() == typeof(MaxLengthAttribute))
                    throw new AuctionSiteArgumentException("Site.UserValidation Error: value too long", $"{e.ValidationResult.MemberNames.First()}", e);
                if (e.ValidationAttribute.GetType() == typeof(RequiredAttribute))
                    throw new AuctionSiteArgumentNullException($" Site.UserValidation Error: {e.ValidationResult.MemberNames.First()} cannot be null", e);
            }

            return newUser;
        }

        public void CreateUser(string username, string password)
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            using (var c = new DbContext(_connectionString))
            {
                var thisSite = c.Sites.SingleOrDefault(s => s.Name == Name);
                if (thisSite == null) throw new AuctionSiteInvalidOperationException("Site Error: This site doesn't exists");
                var users = ToyGetUsers();
                foreach(var u in users)
                {
                    u.Delete();
                }
                c.Remove(thisSite);
                c.SaveChanges();
            }
        }

        public ISession? Login(string username, string password)
        {
            Exists();
            UserValidator(username, password);
            if (username.Length < DomainConstraints.MinUserName)
                throw new AuctionSiteArgumentException("Login.CreateUser Error: username too short", nameof(username));
            if (username.Length > DomainConstraints.MaxUserName)
                throw new AuctionSiteArgumentException("Login.CreateUser Error: username too long", nameof(username));
            if (password.Length < DomainConstraints.MinUserPassword)
                throw new AuctionSiteArgumentException("Login.CreateUser Error: password too short ", nameof(password));

            using (var c = new DbContext(_connectionString))
            {
                var myUser = c.Users!.Include(u => u.Session).SingleOrDefault(u => u.Username == username && u.SiteId == Id && u.Password == Hash.GenerateHash(password));
                if (myUser == null) return null;
                if (myUser.SessionId == null)
                {
                    var newSession = new Session(SiteId, myUser.UserId, 
                        new User(SiteId, username, Hash.GenerateHash(password), _connectionString, _alarmClock), _alarmClock.Now.AddSeconds(SessionExpirationInSeconds), _connectionString, _alarmClock)
                    { Id = myUser.UserId.ToString() };
                    myUser.SessionId = newSession.Id;
                    c.Sessions!.Add(newSession);
                    c.SaveChanges();
                    return newSession;
                }
                else
                {
                    var session = myUser.Session;
                    session!.ValidUntil = _alarmClock.Now.AddSeconds(SessionExpirationInSeconds);
                    c.Update(session);
                    c.SaveChanges();
                    var newSession = new Session(SiteId, myUser.UserId,
                        new User(SiteId, username, Hash.GenerateHash(password), _connectionString, _alarmClock),
                        session.DbValidUntil, _connectionString, _alarmClock)
                    { Id = session.Id};
                    return newSession;
                }


            }
        }

        public DateTime Now()
        {
            return _alarmClock.Now;
        }

        public IEnumerable<IAuction> ToyGetAuctions(bool onlyNotEnded)
        {
            
            Exists();
            var myAuctionsList = new List<IAuction>();
            using(var c = new DbContext(_connectionString))
            {
                List<AuctionTable> auctions = c.Auctions.Where(a => a.SiteId == Id).Include(a => a.CreatedBy).ToList();
                if (onlyNotEnded)
                {
                    foreach (var auction in auctions)
                    {
                        if (auction.EndsOn >= _alarmClock.Now)
                            myAuctionsList.Add(new Auction(auction.AuctionId, auction.SellingUserId, auction.CreatedBy!, SiteId, auction.Description, auction.EndsOn, auction.CurPrice, auction.CurPrice, _connectionString, _alarmClock) { MaximumAmount = auction.MaximumAmount });
                    }
                }
                else
                {
                    foreach (var auction in auctions)
                    {
                        myAuctionsList.Add(new Auction(auction.AuctionId, auction.SellingUserId, auction.CreatedBy!, SiteId, auction.Description, auction.EndsOn, auction.CurPrice, auction.CurPrice, _connectionString, _alarmClock) { MaximumAmount = auction.MaximumAmount });
                    }
                }
            }
        }

        public IEnumerable<ISession> ToyGetSessions()
        {
            IEnumerable<ISession> ToyGetSessions_aux(List<SessionTable> sessions)
            {
                foreach(var session in sessions)
                {
                    yield return new Session(session,new User(session.Owner!, this,_connectionString), this, _connectionString);
                }
            }

            Exists();
            using(var c = new DbContext(_connectionString))
            {
                var sessions = c.Sessions.Where(s => s.SiteId == Id).Include(s => s.Owner).ToList();
                return ToyGetSessions_aux(sessions);
            }
        }

        public IEnumerable<IUser> ToyGetUsers() {
            IEnumerable<IUser> ToyGetUsers_aux(List<UserTable> users)
            {
                foreach (var user in users)
                {
                    yield return new User(user,this,_connectionString);
                }
            }

            Exists();
            using (var c = new DbContext(_connectionString))
            {
                var users = c.Users.Where(s => s.SiteId == Id).ToList();
                return ToyGetUsers_aux(users);
            }
        }

        public void CleanSession()
        {
            using (var c = new DbContext(_connectionString))
            {
                var sessionsToClean =
                    c.Sessions.Where(s => s.SiteId == Id && s.ValidUntil <= Now());

                c.RemoveRange(sessionsToClean);
                c.SaveChanges();
            }
            alarm = _alarmClock.InstantiateAlarm(300000);
        }*/
    }
}
