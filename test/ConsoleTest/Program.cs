using System;
using Dapper;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new ETicketContext())
            {
                var ticketCount = db.Connection.ExecuteScalar<int>("select count(*) from ticket_info where isdel = @Flag", new { Flag = "N" });
                Console.WriteLine($"ticket count {ticketCount}");
            }
            Console.ReadLine();
        }
    }
}
