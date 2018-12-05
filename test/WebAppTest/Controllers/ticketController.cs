using Dapper;
using Microsoft.AspNetCore.Mvc;
using System;

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
        public string GetTicketCount()
        {
            var ticketCount = _dbContxt.Connection.ExecuteScalar<int>("select count(*) from ticket_info where isdel = @Flag", new { Flag = "N" });

            return $"ticket count {ticketCount}";
        }
    }
}