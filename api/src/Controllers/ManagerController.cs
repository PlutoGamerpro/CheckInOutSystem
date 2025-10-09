using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TimeRegistration.Contracts.Requests;
using TimeRegistration.Services;
using TimeRegistration.Filters;

namespace TimeRegistration.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ManagerController : ControllerBase
    {
        private readonly IManagerService _managerService;
        public ManagerController(IManagerService managerService)
        {
            _managerService = managerService;
        }

        [HttpGet("managers")]
        // needs to add [ManagerAuthorize] when filter is fixed
        public IActionResult GetAllManagers()
        {
            var managers = _managerService.GetAllManagers();
            return Ok(managers);
        }

        /*
        LOGIN REMOVIDO/UNIFICADO:
        Agora o login (admin ou manager) deve ser feito via:
            POST /api/admin/login
        Ele tenta admin e se não encontrar tenta manager.
        Este bloco é mantido comentado para referência.
        
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest req)
        {
            try
            {
                var result = _managerService.Login(req);
                return Ok(new { token = result.Token, userName = result.UserName, role = "manager" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound("User not found");
            }
        }
        */

        [HttpPut("user")]
        // needs to add [ManagerAuthorize] when filter is fixed
        public IActionResult UpdateUser([FromBody] UserRecordRequest userRecordRequest)
        {
            try
            {
                _managerService.UpdateUser(userRecordRequest);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound("User not found");
            }
        }
    }
}
            
        

        /*
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
            */
