using System;
using Microsoft.AspNetCore.Mvc;
using Seedwork.Messaging;
using Seedwork.Web.Attributes;
using PortfolioReportsService.Contracts.Commands;
using PortfolioReportsService.Contracts.Events;

namespace PortfolioReportsService.WebApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [NotForProduction]
    public class TestController : Controller
    {
        private readonly IMessageSender _messageSender;

        public TestController(IMessageSender messageSender)
        {
            _messageSender = messageSender;
        }

        /// <summary>
        /// The very test method
        /// </summary>
        /// <returns></returns>
        [HttpPost(nameof(Test))]
        [NotForProduction]
        public IActionResult Test()
        {
            _messageSender.Send(new TestCommand()
            {
                Test = $"TestCommand_{DateTime.Now}"
            });

            _messageSender.Publish(new TestEvent()
            {
                Test = $"TestEvent_{DateTime.Now}"
            });
            return Ok();
        }
    }
}