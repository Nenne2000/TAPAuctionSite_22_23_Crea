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
                var myUser = c.Users.FirstOrDefault(u => u.Username == username && u.SiteId == SiteId && u.Password == password);
                if (myUser == null) return null;
                var mySession = c.Sessions.FirstOrDefault(s => s.UserId == myUser.UserId && s.ValidUntil > Now());
                if (mySession != null) {
                    mySession.ValidUntil = _alarmClock.Now.AddSeconds(SessionExpirationInSeconds);
                    c.Update(mySession);
                    c.SaveChanges();
                    return new Session(mySession.SessionId, mySession.ValidUntil, new User(myUser.UserId, myUser.Username, myUser.Password, _connectionString, this), _connectionString, this);
                } 
                else
                {
                    var newSessionEntity = new SessionTable($"session{Name}{username}{Now()}", Now().AddSeconds(SessionExpirationInSeconds), myUser.UserId, SiteId);
                    c.Sessions.Add(newSessionEntity);
                    c.SaveChanges();
                    return new Session(newSessionEntity.SessionId, newSessionEntity.ValidUntil, new User(myUser.UserId, myUser.Username, myUser.Password, _connectionString, this), _connectionString, this);
                }
            }
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
    }
}
