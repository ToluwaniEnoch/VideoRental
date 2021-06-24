using Api.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using static Api.Models.Constants.ResponseCodes;

namespace Api.Controllers
{
    /// <summary>
    /// Base Controller class
    /// </summary>
    public abstract class BaseController : Controller
    {
        /// <summary>
        /// Handle Response Method
        /// </summary>
        /// <typeparam name="TRes"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        protected ActionResult<TRes> HandleResponse<TRes>(TRes result) where TRes : StatusResponse
        {
            if (result.Code == NoPermission) return Unauthorized(result);
            if (result.Code == NoData) return NotFound(result);
            if (result.Code == Success) return Ok(result);
            return BadRequest(result);
        }
    }
}