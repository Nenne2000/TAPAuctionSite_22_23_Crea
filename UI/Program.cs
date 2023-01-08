using Microsoft.Data.SqlClient;
using TAP22_23.AuctionSite.Interface;
using Crea;
class Program
{
    static void Main(string[] args)
    {
        try
        {
            using (var c =
                new DbContext(
                    @"Data Source=.;Initial Catalog=ziopera;Integrated Security=True;"))
            {
                c.Database.EnsureDeleted();
                c.Database.EnsureCreated();
            }
        }
        catch (SqlException e)
        {
            throw new AuctionSiteUnavailableDbException("Unavailable DB", e);
        }


        Console.WriteLine("Hello World!");
    }
}