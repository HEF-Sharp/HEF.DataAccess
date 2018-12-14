using System;
using System.Threading.Tasks;
using Dapper;

namespace ConsoleTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using (var db = new ETicketContext())
            {
                var ticketCount = await db.Connection.ExecuteScalarAsync<int>("select count(*) from ticket_info where isdel = @Flag", new { Flag = "N" });
                Console.WriteLine($"ticket count {ticketCount}");

                var moduleCount = await db.Connection.ExecuteScalarAsync<int>("select count(*) from module_info");
                Console.WriteLine($"module count {moduleCount}");
            }
            Console.ReadLine();
        }
    }
}
