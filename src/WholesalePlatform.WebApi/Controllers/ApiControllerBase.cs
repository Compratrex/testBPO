using Microsoft.AspNetCore.Mvc;
using WholesalePlatform.WebApi.Contracts.Common;

namespace WholesalePlatform.WebApi.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected ActionResult<ApiResponse<T>> OkResponse<T>(T data)
    {
        return Ok(ApiResponse<T>.Success(data));
    }

    protected ActionResult<ApiResponse<T>> CreatedResponse<T>(string location, T data)
    {
        return Created(location, ApiResponse<T>.Success(data));
    }
}
