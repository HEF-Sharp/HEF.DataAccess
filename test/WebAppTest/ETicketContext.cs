using HEF.Data;

namespace WebAppTest
{
    public class ETicketContext : DbContext
    {
        public ETicketContext(DbContextOptions options)
            : base(options)
        { }
    }
}
