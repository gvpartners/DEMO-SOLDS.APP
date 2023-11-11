using DEMO_SOLDS.APP.Models;
using DEMO_SOLDS.APP.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using R2.DEMO.APP.Context;

namespace DEMO_SOLDS.APP.Controllers
{
    [EnableCors("corsRules")]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : Controller
    {
        private CustomerService _customerService;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext context;

        public CustomerController(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            this.context = context;
            _customerService = new CustomerService(configuration, context);
        }

        [HttpGet("GetAllCustomers")]
        public async Task<ActionResult<dynamic>> GetAllCustomers()
        {
            dynamic response;
            return await Task.Run(() =>
            {
                try
                {
                    response = _customerService.GetAllCustomers();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                return Ok(response);
            });
        }

        [HttpPost("CreateCustomer")]
        public async Task<ActionResult<dynamic>> CreateCustomer(Customers customer)
        {
            dynamic response;
            return await Task.Run(() =>
            {
                try
                {
                    response = _customerService.CreateCustomer(customer.IdentificationType, customer.IdentificationInfo, customer.CustomerName);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                return Ok(response);
            });
        }

        [HttpPut("UpdateCustomer/{customerId}")]
        public async Task<ActionResult<dynamic>> UpdateCustomer(Guid customerId,Customers customer)
        {
            dynamic response;
            return await Task.Run(() =>
            {
                try
                {
                    response = _customerService.UpdateCustomer(customerId, customer.IdentificationType, customer.IdentificationInfo,customer.CustomerName);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                return Ok(response);
            });
        }

        [HttpPut("RemoveCustomer/{id}")]
        public async Task<IActionResult> RemoveCustomer(Guid id)
        {
            try
            {
                await Task.Run(() =>
                {
                    _customerService.RemoveCustomer(id);
                });

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
