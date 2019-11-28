﻿using JWTDemo.Server.Entity;
using Microsoft.Extensions.Options;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace JWTDemo.Server.TokenInfo
{
    public class TokenHelper : ITokenHelper
    {
        private IOptions<JWTConfig> _options;

        public TokenHelper(IOptions<JWTConfig> options)
        {
            _options = options;
        }
        public Token CreateToken(User user)
        {
            Claim[] claims = new Claim[] { new Claim(ClaimTypes.Name, user.UserCode), new Claim(ClaimTypes.Name, user.UserName) };

            return CreateToken(claims);
        }

        private Token CreateToken(Claim[] claims)
        {
            var now = DateTime.Now;
            var expires = now.Add(TimeSpan.FromMinutes(_options.Value.AccessTokenExpiresMinutes));
            var token = new JwtSecurityToken(
                issuer: _options.Value.Issuer,
                audience: _options.Value.Audience,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.IssuerSigningKey)), SecurityAlgorithms.HmacSha256));
            return new Token { TokenContent = new JwtSecurityTokenHandler().WriteToken(token), TokenExpiresTime = expires };
        }
    }
}
