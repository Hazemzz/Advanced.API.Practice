using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Business;

namespace Advanced.API.Practice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly JwtGenerator _jwtGenerator;

        public TokenController(JwtGenerator jwtGenerator)
        {
            _jwtGenerator = jwtGenerator;
        }

        [HttpGet("generate")]
        public Task<string> GetToken()
        {
            return _jwtGenerator.Generate("anything");
        }

        [HttpGet("decode")]
        public string DecodeToken(string token)
        {
            var principal = _jwtGenerator.DecodeToken(token);
            var userName = principal.Claims.First(x => x.Type == ClaimTypes.Name).Value;
            return userName;
        }
    }
}
