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

        [HttpGet("getall")]
        public IActionResult All()
        {
            var Userid = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value);

            List<EventModel> events = Db.Events.Where(x => x.OwnerId == Userid)
                .Select(s => new EventModel() 
                {
                    Id=s.Id,
                    Data=s.Data,
                    Time=s.Time
                })
                .ToList();
            
            return Ok(events);
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
            return Ok(NewEvent.Id);
        }

        [HttpPut("update")]
        async public Task<IActionResult> UpdateEvent([FromBody] EventModel UpdateEventModel)
        {
            var Userid = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value);

            Event UpdateEvent = new Event()
            {
                Id = UpdateEventModel.Id,
                Time = UpdateEventModel.Time,
                Data = UpdateEventModel.Data,
                OwnerId = Userid
            };
            Db.Entry(UpdateEvent).State=EntityState.Modified;
            await Db.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("delete")]
        async public Task<IActionResult> DeleteEvent([FromQuery] Guid EventId)
        {
            var DeletedEvent= await Db.Events.FirstOrDefaultAsync(x => x.Id == EventId);
            if (DeletedEvent != null)
            {
                Db.Entry(DeletedEvent).State = EntityState.Deleted;
                await Db.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return BadRequest();   
            }
        }
    }
}
