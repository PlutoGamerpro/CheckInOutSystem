using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TimeRegistration.Classes;
using TimeRegistration.Models;
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
        [ManagerAuthorize]
        public IActionResult GetAllManagers()
        {
            var managers = _managerService.GetAllManagers();
            return Ok(managers);
        }
       
        [HttpPost("login")]
        public IActionResult Login([FromBody] AdminLoginRequest req)
        {
            try
            {
                var result = _adminservice.Login(req);
                // Retorna o token no body para o frontend salvar e usar no header X-Admin-Token
                return Ok(new { token = result.Token, userName = result.UserName });
            }
            catch (KeyNotFoundException)
            {
                return NotFound("User not found");
            }

        }




       [HttpPut("user")]
       [ManagerAuthorize]
       /*[AdminAuthorize]*/ // could return a token instead change call to take record 
        public IActionResult UpdateUser([FromBody] UserRecordRequest userRecordRequest /* int id, User user*/)
        {
            try
            {
                _managerService.UpdateUser(userRecordRequest);
              //  _adminservice.UpdateUser(userRecordRequest);
                //  _adminservice.UpdateUser(id, user);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound("User not found");
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
    }
}
