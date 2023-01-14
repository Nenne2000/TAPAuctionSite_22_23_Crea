using Microsoft.Data.SqlClient;
using TAP22_23.AlarmClock.Interface;

namespace Crea
{
    public class HostFactory : IHostFactory
    {
        public void CreateHost(string connectionString)
        {
            if (null == connectionString || connectionString.Length == 0)
                throw new AuctionSiteArgumentNullException("HostFactory.CreateHost Error: Connection string cannot be null or empty");

            using (var c = new DbContext(connectionString))
            {
                try
                {
                    c.Database.EnsureDeleted();
                    c.Database.EnsureCreated();
                }
                catch (SqlException e)
                {
                    throw new AuctionSiteUnavailableDbException("HostFactory.CreateHost Error: no db found", e);
                }
            }
        }

        public IHost LoadHost(string connectionString, IAlarmClockFactory alarmClockFactory)
        {
            if (null == connectionString || connectionString.Length == 0)
                throw new AuctionSiteArgumentNullException("HostFactory.LoadHost Error: connection string cannot be null or empty");
            if (null == alarmClockFactory)
                throw new AuctionSiteArgumentNullException("HostFactory.LoadHost Error: alarmClockFactory cannot be null");

            using (var c = new DbContext(connectionString))
            {
                if (!c.Database.CanConnect()) throw new AuctionSiteUnavailableDbException("HostFactory.LoadHost Error: no database available");
                return new Host(connectionString, alarmClockFactory);
            }
        }
    }
}
