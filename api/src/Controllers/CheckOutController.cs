using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TimeRegistration.Classes;
using TimeRegistration.Interfaces;
using TimeRegistration.Services;

namespace TimeRegistration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckOutController : ControllerBase
    { 
        private readonly ICheckOutService _checkoutservice;

        public CheckOutController(ICheckOutService checkoutservice)
        {
            _checkoutservice = checkoutservice;
        }
     
        [HttpGet]
        public ActionResult<IEnumerable<CheckOut>> GetAll()
        {
            var checkouts = _checkoutservice.GetAllCheckOuts();
            return checkouts != null ? Ok(checkouts) : NotFound();
        }

        [HttpGet("{id}")]
        public ActionResult<CheckOut> Get(int id)
        {
            var checkout = _checkoutservice.GetCheckOutById(id);
            return checkout != null ? Ok(checkout) : NotFound();           
        }
        
        [HttpPost("byphone/{tlf}")]
        public IActionResult CheckOutByPhone(string tlf)
        {
            var checkout = _checkoutservice.CreateCheckOut(tlf);
            return CreatedAtAction(nameof(Get), new { id = checkout.CheckOutId, checkout.Name, checkout.Phone }, checkout);                        
        }

        [HttpPut("{id}")]
        public ActionResult<CheckOut> Update(int id, CheckOut checkOut)
        {
            var updated = _checkoutservice.UpdateCheckOut(id, checkOut);
            if (updated == null)
                return NotFound();
            return AcceptedAtAction(nameof(Get), new { id = updated.Id }, updated);
        }

        [HttpDelete("{id}")]
        public ActionResult<CheckOut> Delete(int id)
        {
            try
            {
                _checkoutservice.DeleteCheckOut(id);
                return NoContent();
            }
            catch (Exception)
            {
                return NotFound();
            }
           
        }
    }
}