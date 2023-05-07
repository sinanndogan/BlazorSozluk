﻿using AutoMapper;
using BlazorSozluk.Api.Application.Interfaces.Repositories;
using BlazorSozluk.Common.Infrastructure;
using BlazorSozluk.Common.Infrastructure.Exceptions;
using BlazorSozluk.Common.Models.Queries;
using BlazorSozluk.Common.Models.RequestModels;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BlazorSozluk.Api.Application.Features.Commands.User.Login;

//burada bu iki parametreyi alacak requesthandler  usercommand alıp viewmodel dönecek demiş oluyoruz burda 
public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginUserViewModel>
{
    private readonly IUserRepository userRepository;
    private readonly IMapper mapper;
    private readonly IConfiguration configuration;

    public LoginUserCommandHandler(IUserRepository userRepository, IMapper mapper, IConfiguration configuration)
    {
        this.userRepository = userRepository;
        this.mapper = mapper;
        this.configuration = configuration;
    }

    public async Task<LoginUserViewModel> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        //Kullanıcı var mı yok mu diye bakıyoruz ilk adım 

        var dbUser = await userRepository.GetSingleAsync(i => i.EmailAddress == request.EmailAddress);
        if (dbUser == null)
        {
            throw new DatabaseValidationException("User Not Found!");
        }

        //Diyelim ki user var ama şifresi yanlış - Dışarıdan gelen password ile db 'de ki karşılaştırılmalı 

        var pass = PasswordEncryptor.Encrpt(request.Password);
        if (dbUser.Password != pass)
        {
            throw new DatabaseValidationException("Password is Wrong");
        }

        //Sisteme kulanıcı kayıt olmuş olabilir ama Email Confirm edilmişmi onaylama işlemi yapılmıssa devam edebilsin

        if (!dbUser.EmailConfirmed)
        {
            throw new DatabaseValidationException("Email address is not confirmed yet!");
        }

        var result = mapper.Map<LoginUserViewModel>(dbUser);

        var claims = new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier,dbUser.Id.ToString()),
            new Claim(ClaimTypes.Email,dbUser.EmailAddress.ToString()),
            new Claim(ClaimTypes.Name,dbUser.UserName.ToString()),
            new Claim(ClaimTypes.GivenName,dbUser.FirstName.ToString()),
            new Claim(ClaimTypes.Surname,dbUser.LastName.ToString()),
        };

        result.Token = GenerateToken(claims);

        return result;

    }

    private string GenerateToken(Claim[] claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["AuthConfig:Secret"]));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiry = DateTime.Now.AddDays(10);

        var token = new JwtSecurityToken(claims: claims, expires: expiry, signingCredentials: creds, notBefore: DateTime.Now);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}