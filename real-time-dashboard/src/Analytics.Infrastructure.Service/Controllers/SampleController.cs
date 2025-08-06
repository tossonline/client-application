// Copyright (c) DigiOutsource. All rights reserved.

using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Analytics.Infrastructure.Service.Controllers
{
    [ApiController]
    [ApiVersion(1.0)]
    [Route("api/[controller]")]
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