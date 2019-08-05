using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace vietjet_series_booking_dotnet.App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        const int OK = (int)HttpStatusCode.OK;
        const int CREATED = (int)HttpStatusCode.Created;
        const int NO_CONTENT = (int)HttpStatusCode.NoContent;

        const int BAD_REQUEST = (int)HttpStatusCode.BadRequest;
        const int UNAUTHORIZED = (int)HttpStatusCode.Unauthorized;
        const int FORBIDDEN = (int)HttpStatusCode.Forbidden;
        const int CONFLICT = (int)HttpStatusCode.Conflict;

        const int INTERNAL_SERVER_ERROR = (int)HttpStatusCode.InternalServerError;

        protected IActionResult ResponseData(object _data = null, string _message = "", int _status_code = 200)
        {
            return StatusCode(_status_code, new { data = _data, message = _message, status_code = _status_code });
        }
        protected IActionResult ResponseOk(dynamic _data = null, string _message = null)
        {
            return StatusCode(OK, new { data = _data, message = _message, status_code = OK });
        }

        protected IActionResult ResponseCreated(dynamic _data = null, string _message = null)
        {
            return StatusCode(CREATED, new { data = _data, message = _message, status_code = CREATED });
        }

        protected IActionResult ResponseNoContent()
        {
            return StatusCode(NO_CONTENT);
        }

        protected IActionResult ResponseBadRequest(string _message = null)
        {
            return StatusCode(BAD_REQUEST, new { message = _message, status_code = BAD_REQUEST });
        }

        protected IActionResult ResponseUnauthorized(string _message = null)
        {
            return StatusCode(UNAUTHORIZED, new { message = _message, status_code = UNAUTHORIZED });
        }

        protected IActionResult ResponseForbidden(string _message = null)
        {
            return StatusCode(FORBIDDEN, new { message = _message, status_code = FORBIDDEN });
        }

        protected IActionResult ResponseConflict(string _message)
        {
            return StatusCode(CONFLICT, new { message = _message, Status_code = CONFLICT });
        }
        protected IActionResult ResponseInternalServerError(string _message = null)
        {
            return StatusCode(INTERNAL_SERVER_ERROR, new { message = _message, status_code = INTERNAL_SERVER_ERROR });
        }
    }
}
