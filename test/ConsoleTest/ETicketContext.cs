using HEF.Data;

namespace ConsoleTest
{
    public class ETicketContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("Data Source=192.168.11.100;Initial Catalog=eticketdb2.0;uid=root;pwd=123456;");
        }
    }
}
