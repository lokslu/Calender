using Calendar.EF;
using Calendar.EF.Entities;
using Calendar.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calendar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventController : ControllerBase
    {
        private readonly ApplicationContext Db;

        public EventController(ApplicationContext Db)
        {
            this.Db = Db;

        }

        [HttpPost("add")]
        async public Task<IActionResult> AddEvent([FromBody] EventModel NewEventModel)
        {
            var Userid = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value);
           
            Event NewEvent = new Event()
            {
                Time = NewEventModel.Time,
                Data = NewEventModel.Data,
                OwnerId = Userid
            };
            await Db.Events.AddAsync(NewEvent);
            await Db.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("update")]
        async public Task<IActionResult> UpdateEvent([FromBody] EventModel UpdateEventModel)
        {
            var Userid = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value);
         
            Event UpdateEvent = new Event()
            {
                Time = UpdateEventModel.Time,
                Data = UpdateEventModel.Data,
                OwnerId = Userid
            };
            Db.Entry(UpdateEvent).State=EntityState.Modified;
            await Db.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("delete")]
        async public Task<IActionResult> DeleteEvent([FromBody] Guid EventId)
        {
            var DeletedEvent= await Db.Events.FirstOrDefaultAsync(x => x.Id == EventId);
            Db.Entry(DeletedEvent).State = EntityState.Deleted;
            await Db.SaveChangesAsync();
            return Ok();
        }
    }
}
