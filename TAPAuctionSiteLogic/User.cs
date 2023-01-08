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
        public string Username => throw new NotImplementedException();

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IAuction> WonAuctions()
        {
            throw new NotImplementedException();
        }
    }
}
