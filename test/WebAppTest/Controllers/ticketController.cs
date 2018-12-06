using Dapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace WebAppTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ticketController : ControllerBase
    {
        private readonly ETicketContext _dbContxt;

        public ticketController(ETicketContext dbContext)
        {
            _dbContxt = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        [HttpGet("count")]
        public async Task<string> GetTicketCount()
        {
            var ticketCount = await _dbContxt.Connection.ExecuteScalarAsync<int>("select count(*) from ticket_info where isdel = @Flag", new { Flag = "N" });

            return $"ticket count {ticketCount}";
        }
    }
}