using DEMO_SOLDS.APP.Models;
using DEMO_SOLDS.APP.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using R2.DEMO.APP.Context;
using Microsoft.AspNetCore.Authorization;
using DEMO_SOLDS.APP.Models.Pagination;

namespace DEMO_SOLDS.APP.Controllers
{
    [EnableCors("corsRules")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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

        [HttpPost("GetAllCustomers")]
        public async Task<ActionResult<dynamic>> GetAllCustomers(CustomerPage pag)
        {
            dynamic response;
            return await Task.Run(() =>
            {
                try
                {
                    response = _customerService.GetAllCustomers(pag);
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
                    response = _customerService.CreateCustomer(customer);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                return Ok(response);
            });
        }
        [HttpGet("GetIsCustomerInDb/{customerNumber}")]
        public async Task<ActionResult<dynamic>> GetIsCustomerInDb(string customerNumber)
        {
            dynamic response;
            return await Task.Run(() =>
            {
                try
                {
                    response = _customerService.GetIsCustomerInDb(customerNumber);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                return Ok(response);
            });
        }
        [HttpGet("GetCustomerAddress/{documentInfo}")]
        public async Task<ActionResult<dynamic>> GetCustomerAddress(string documentInfo)
        {
            dynamic response;
            return await Task.Run(() =>
            {
                try
                {
                    response = _customerService.GetCustomerAddress(documentInfo);
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
                    response = _customerService.UpdateCustomer(customerId, customer);
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
