using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAP22_23.AuctionSite.Interface;

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

            throw new NotImplementedException();
        }

        public IEnumerable<IAuction> WonAuctions()
        {
            throw new NotImplementedException();
        }
    }
}
