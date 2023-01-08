﻿using Microsoft.Data.SqlClient;
using TAP22_23.AlarmClock.Interface;
using TAP22_23.AuctionSite.Interface;

namespace Crea
{
    public class HostFactory : IHostFactory
    {
        public void CreateHost(string connectionString)
        {
            if (null == connectionString || connectionString.Length == 0)
                throw new AuctionSiteArgumentNullException("HostFactory.CreateHost Error: Connection string cannot be null or empty");

            using(var c = new DbContext(connectionString))
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
            if(null == alarmClockFactory)
                throw new AuctionSiteArgumentNullException("HostFactory.LoadHost Error: alarmClockFactory cannot be null");

            using (var c = new DbContext(connectionString))
            {
                if (!c.Database.CanConnect()) throw new AuctionSiteUnavailableDbException("HostFactory.LoadHost Error: no database available");
                return new Host(connectionString, alarmClockFactory);
            }
        }
    }

    public class Host : IHost
    {
        public string ConnectionString { get; set; }
        public IAlarmClockFactory AlarmClockFactory { get; set; }

        public Host(string connectionString, IAlarmClockFactory alarmClockFactory)
        {
            ConnectionString = connectionString;
            AlarmClockFactory = alarmClockFactory;
        }

        public void CreateSite(string name, int timezone, int sessionExpirationTimeInSeconds, double minimumBidIncrement)
        {
            //Controlli-----------------------------------------------------------------------------------------------------------------
            if(null == name) throw new AuctionSiteArgumentNullException("Host.CreateSite Error: site name cannot be null");
            if (name.Length < DomainConstraints.MinSiteName || name.Length > DomainConstraints.MaxSiteName)
                throw new AuctionSiteArgumentException("Host.CreateSite Error: Site Name too short or too long");
            if(timezone < DomainConstraints.MinTimeZone || timezone > DomainConstraints.MaxTimeZone)
                throw new AuctionSiteArgumentOutOfRangeException(nameof(timezone), timezone,
                    "Host.CreateSite Error: timezone too short or too long");
            if (sessionExpirationTimeInSeconds <= 0)
                throw new AuctionSiteArgumentOutOfRangeException(nameof(sessionExpirationTimeInSeconds), sessionExpirationTimeInSeconds,
                    "Host.CreateSite Error: expiration time must be positive");
            if (minimumBidIncrement <= 0)
                throw new AuctionSiteArgumentOutOfRangeException(nameof(minimumBidIncrement), minimumBidIncrement,
                    "Host.CreateSite Error: minimum bid increment must be positive");
            //---------------------------------------------------------------------------------------------------------------------------

            var s = new SiteTable(name, timezone, sessionExpirationTimeInSeconds, minimumBidIncrement);
            using(var c = new DbContext(ConnectionString))
            {
                var oldSite = c.Sites.SingleOrDefault(s => s.Name == name);
                if (oldSite != null)
                    throw new AuctionSiteNameAlreadyInUseException(name, "Host.CreateSite Error: This name is already used for another site");
                //??????????????????
                try
                {
                    c.Sites.Add(s);
                    c.SaveChanges();
                }
                catch (SqlException e)
                {
                    throw new AuctionSiteUnavailableDbException("Host.CreateSite Error: generic database error",e);
                }

            }
        }

        public IEnumerable<(string Name, int TimeZone)> GetSiteInfos()
        {
            List<SiteTable> sites;
            using(var c = new DbContext(ConnectionString))
            {
                try
                {
                    sites = c.Sites.ToList();
                }
                catch(SqlException e)
                {
                    throw new AuctionSiteUnavailableDbException("Host.GetSiteInfos Error: generic database error",e);
                }
                foreach(var s in sites)
                {
                    yield return (s.Name, s.Timezone);
                }
            }
        }

        public ISite LoadSite(string name)
        {
            //controlli-----------------------------------------------------------------------------------------------------
            if (null == name) throw new AuctionSiteArgumentNullException("Host.LoadSite Error: site name cannot be null");
            if (name.Length < DomainConstraints.MinSiteName || name.Length > DomainConstraints.MaxSiteName)
                throw new AuctionSiteArgumentException("Host.LoadSite Error: Site Name too short or too long");
            //--------------------------------------------------------------------------------------------------------------
            using(var c = new DbContext(ConnectionString))
            {
                try
                {
                    var site = c.Sites.FirstOrDefault(s => s.Name == name);
                    if (site != null)
                        return new Site(site, ConnectionString, AlarmClockFactory.InstantiateAlarmClock(site.Timezone));
                }
                catch(SqlException e)
                {
                    throw new AuctionSiteUnavailableDbException("Host.LoadSite Error: generic database error", e);
                }
                throw new AuctionSiteInexistentNameException(name,"Host.LoadSite Error: inexistent site name");
            }
        }
    }
}