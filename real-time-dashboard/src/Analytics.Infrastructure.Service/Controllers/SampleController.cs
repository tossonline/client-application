// Copyright (c) DigiOutsource. All rights reserved.

using Microsoft.AspNetCore.Mvc;

namespace Analytics.Infrastructure.Service.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class SampleController : ControllerBase
    {
        [HttpPost]
        [Route("ping")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public string Ping()
        {
            return "Pong";
        }
    }
}