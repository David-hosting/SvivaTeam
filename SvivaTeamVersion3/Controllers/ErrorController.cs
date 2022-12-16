using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SvivaTeamVersion3.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            this.logger = logger;
        }

        // GET: /<controller>/
        [AllowAnonymous]
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Sorry, the resource you requested could not be found.";
                    logger.LogWarning($"404 Error Occured. Path: {statusCodeResult.OriginalPath}" + 
                        $"and QueryString {statusCodeResult.OriginalQueryString}");
                    break;
                case 405:
                    ViewBag.ErrorMessage = "Method not allowed.";
                    logger.LogWarning($"405 Error Occured. Path: {statusCodeResult.OriginalPath}" +
                        $"and QueryString {statusCodeResult.OriginalQueryString}");
                    break;
            }

            return View("NotFound");
        }

        [AllowAnonymous]
        [Route("Error")]
        public IActionResult Error()
        {
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            logger.LogError($"The part {exceptionDetails.Path} threw an exception " + 
                $"{exceptionDetails.Error}");

            return View("Error");
        }
    }
}
