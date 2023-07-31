using FluantValidationDemoApp.Data;
using FluantValidationDemoApp.DTOs;
using FluantValidationDemoApp.Validations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FluantValidationDemoApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ApplicationDBContext _applicationDBContext;

        public CustomerController( ApplicationDBContext applicationDBContext)
        {
            _applicationDBContext = applicationDBContext;
        }
        [HttpPost("AddNewCustomer",Order =0)]
        public IActionResult Add(CustomerDTO customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);               
            }

            _applicationDBContext.Add(customer);
            _applicationDBContext.SaveChanges();

            return Ok();
        }

        [HttpPost("UpdateNewCustomer", Order = 1)]
        public IActionResult Update(CustomerDTO customer)
        {
            CustomerValidator validator = new CustomerValidator();
            var validationResult = validator.Validate(customer);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            _applicationDBContext.Update(customer);
            _applicationDBContext.SaveChanges();

            return Ok();
        }
    }
}
