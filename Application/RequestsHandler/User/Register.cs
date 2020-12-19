﻿using Application.Interfaces;
using Application.Models;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.RequestsHandler.User
{
    public class Register
    {
        public class AccountRegister : IRequest<AuthUserDTO>
        {
           
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
        }
        public class Validator : AbstractValidator<AccountRegister>
        {
            public Validator()
            {
                RuleFor(x => x.FirstName).NotEmpty().WithMessage("Your name is required").MinimumLength(3).WithMessage("Min length is 3").MaximumLength(10).WithMessage("Max length is 10");
                RuleFor(x => x.LastName).NotEmpty().WithMessage("Your name is required").MinimumLength(3).WithMessage("Min length is 3").MaximumLength(10).WithMessage("Max length is 10");
                RuleFor(x => x.UserName).NotEmpty().WithMessage("Your UserName is required").MinimumLength(5).WithMessage("Min length is 5").MaximumLength(18).WithMessage("Max length is 18").Matches("^[a-z]([a-zA-z0-9_]){5,18}$").WithMessage("Must start with lower case and letter, numbers and underscore are allowed with minimum length 5 and maxlenght of 18");

                RuleFor(x => x.Password).NotEmpty().WithMessage("Your password is required").MinimumLength(6).WithMessage("Min length is 6");
            }
        }
        public class Handler : IRequestHandler<AccountRegister, AuthUserDTO>
        {
            private readonly DataContext dataContext;
            private readonly UserManager<AppUser> userManager;
            private readonly IAuthCookies authCookies;
            private readonly IRefreshTokenGenerator refreshTokenGenerator;

            public Handler(DataContext dataContext, UserManager<AppUser> userManager,IAuthCookies authCookies,IRefreshTokenGenerator refreshTokenGenerator)
            {
                this.dataContext = dataContext;
                this.userManager = userManager;
                this.authCookies = authCookies;
                this.refreshTokenGenerator = refreshTokenGenerator;
            }
            public async Task<AuthUserDTO> Handle(AccountRegister request, CancellationToken cancellationToken)
            {
                var isExist = await dataContext.Users.FirstOrDefaultAsync(x => x.UserName == request.UserName) != null;

                if (isExist)
                    throw new HttpContextException(HttpStatusCode.BadRequest , new { User = "Username is already exist" });

                var user = new AppUser
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    UserName = request.UserName
                };
                var registerResult = await userManager.CreateAsync(user, request.Password);
                var roleResult = await userManager.AddToRoleAsync(user, "Normal");
                if (registerResult.Succeeded && roleResult.Succeeded)
                {
                    var refreshToken = refreshTokenGenerator.Generate(user.UserName);
                    await dataContext.RefreshTokens.AddAsync(new RefreshToken { Token = refreshToken, CreatedAt = DateTime.UtcNow, AppUser = user, ExpireAt = DateTime.UtcNow.AddDays(2) });
                    var success = await dataContext.SaveChangesAsync() > 0;
                    if (success)
                    {
                        await authCookies.SendAuthCookies(user, refreshToken);
                        return new AuthUserDTO(user);
                    }

                }
                throw new Exception("Server Error - Register");
            }
        }
    }
}