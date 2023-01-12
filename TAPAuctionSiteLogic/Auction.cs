using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAP22_23.AlarmClock.Interface;
using TAP22_23.AuctionSite.Interface;

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

        public bool Bid(ISession session, double offer)
        {
            throw new NotImplementedException();
        }

        public double CurrentPrice()
        {
            throw new NotImplementedException();
        }

        public IUser? CurrentWinner()
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }
    }
}
