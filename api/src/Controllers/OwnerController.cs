using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TimeRegistration.Classes;
using TimeRegistration.Models;
using TimeRegistration.Services;

namespace TimeRegistration.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OwnerController : ControllerBase
    {
        private readonly IOwnerService _ownerService;
        public OwnerController(IOwnerService ownerService)
        {
            _ownerService = ownerService;
        }


        [HttpDelete("delete-admin")]
        public IActionResult DeleteAdmin([FromBody] DeleteAdminRecord record)
        {
            try
            {
                _ownerService.DeleteAdmin(record);
                // _ownerService.DeleteAdmin(userRecord.Id, userRecord);
                return Ok("Admin deleted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
