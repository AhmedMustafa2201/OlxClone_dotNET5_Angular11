﻿using Application.Models;
using Application.RequestsHandler.User;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMediator mediator;

        public UserController(IMediator mediator)
        {
            this.mediator = mediator;
        }
        //localhost/api/user/register
        [HttpPost("register")]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult<AuthUserDTO>> Register(Register.Command register)
        {
            return await mediator.Send(register);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult<AuthUserDTO>> Login(Login.Command login)
        {
            return await mediator.Send(login);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("forgery")]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult<Unit>> GenerateForgeryToken()
        {
            return await mediator.Send(new ForgeryToken.Generate());
        }
        [AllowAnonymous]
        [HttpGet("refresh")]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult<AuthUserDTO>> Refresh()
        {
            return await mediator.Send(new TokenRefresh.Query());
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("logout")]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult<bool>> Logout()
        {
            return await mediator.Send(new Logout.Query());
        }
    }
}
