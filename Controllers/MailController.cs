using BasicWebApi.Data.Repository.Interface;
using BasicWebApi.Models;
using EASendMail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BasicWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        public ILogger<MailController> _logger { get; }

        private readonly IMailSender _mail;

        public MailController(ILogger<MailController> logger, IMailSender mail) {
            _logger = logger;
            _mail = mail;
        }

        [HttpPost]
        public async Task<IActionResult> Index(Mails myEmail)
        {
            if (myEmail is null)
            {
                return BadRequest("No email address is provided.");
            }
            else 
            {
                try
                {
                    await _mail.SendMail(myEmail.Email);
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    return StatusCode(500, "Something went wrong.");
                }
            }
            
        }
    }
}
