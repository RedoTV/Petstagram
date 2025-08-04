using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Petsgram.Application.Generators;
using Petsgram.Application.Interfaces.Auth;
using Petsgram.Application.Settings;
using Petsgram.Domain.Entities;

namespace Petsgram.Infrastructure.Services.Auth;

public class AuthService : IAuthService
{

    private readonly ITokenGenerator _tokenGenerator;

    public AuthService(ITokenGenerator tokenGenerator)
    {
        _tokenGenerator = tokenGenerator;
    }


}