using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAP22_23.AuctionSite.Interface;

namespace Crea
{
    public class Auction : IAuction
    {
        public int Id => throw new NotImplementedException();

        public IUser Seller => throw new NotImplementedException();

        public string Description => throw new NotImplementedException();

        public DateTime EndsOn => throw new NotImplementedException();

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
