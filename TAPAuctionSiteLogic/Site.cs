using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAP22_23.AlarmClock.Interface;
using TAP22_23.AuctionSite.Interface;

namespace Crea
{
    public class Site : ISite
    {
        public Site(SiteTable site, string connectionString, IAlarmClock alarmClock)
        {
        }

        public string Name => throw new NotImplementedException();

        public int Timezone => throw new NotImplementedException();

        public int SessionExpirationInSeconds => throw new NotImplementedException();

        public double MinimumBidIncrement => throw new NotImplementedException();

        public void CreateUser(string username, string password)
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public ISession? Login(string username, string password)
        {
            throw new NotImplementedException();
        }

        public DateTime Now()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IAuction> ToyGetAuctions(bool onlyNotEnded)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ISession> ToyGetSessions()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IUser> ToyGetUsers()
        {
            throw new NotImplementedException();
        }
    }
}
