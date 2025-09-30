using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TimeRegistration.Filters;
using TimeRegistration.Services;
using TimeRegistration.Contracts.Requests;

namespace TimeRegistration.Controllers
{
    [ApiController]
    [Route("api/external")]
    public class ExternalController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public ExternalController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpDelete("user/{id}")]
        [AdminAuthorize]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                _adminService.DeleteUser(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound("User not found");
            }
        }

        // Atualização de usuário vinda do dashboard (PUT /api/external/user)


        [HttpPut("user")]
        [AdminAuthorize]
        public IActionResult UpdateUser([FromBody] UserRecordRequest userRecordRequest)
        {
            if (userRecordRequest == null || userRecordRequest.Id <= 0)
                return BadRequest("Invalid payload");
            try
            {
                _adminService.UpdateUser(userRecordRequest);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound("User not found");
            }
            catch (Exception)
            {
                return StatusCode(500, "Update failed");
            }
        }
    }
}
