﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAP22_23.AuctionSite.Interface;

namespace Crea
{
    public class Session : ISession
    {
        public string Id => throw new NotImplementedException();

        public DateTime ValidUntil => throw new NotImplementedException();

        public IUser User => throw new NotImplementedException();

        public IAuction CreateAuction(string description, DateTime endsOn, double startingPrice)
        {
            throw new NotImplementedException();
        }

        public void Logout()
        {
            throw new NotImplementedException();
        }
    }
}