using Core.Application.Interface.Token;
using Core.Domain.Entities;
using Infrastructure.Constants;
using Infrastructure.DotEnv;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.  Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Token;

public class TokenServices : ITokenServices
{
  

    public string BuildToken(string key, string issuer, string audience, UserEntity user, IList<string> userRoles)
    {
        var claims = new List<Claim>
{
    new Claim(ClaimTypes.Name, user.UserName!),
    new Claim(ClaimTypes.Email, user.Email!),
    new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
};

        foreach (var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
        double tokenLifetime = InfrastructureConstants.TOKEN_EXPIRY_DURATION_DAYS;
        double.TryParse(VariableBuilder.GetVariable(EnvFileConstants.ACCESS_TOKEN_LIFETIME_IN_DAYS), out tokenLifetime);
        var tokenDescriptor = new JwtSecurityToken(issuer, audience, claims,
            expires: DateTime.Now.AddDays(tokenLifetime),
            signingCredentials: credentials);
        string res = string.Empty;
        try
        {
            res = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
        catch (Exception ex)
        {

        }
        return res;
    }
    public string BuildToken(string key, string issuer, string audience, UserEntity user, Claim[] claims)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
        double tokenLifetime = InfrastructureConstants.TOKEN_EXPIRY_DURATION_DAYS;
        double.TryParse(VariableBuilder.GetVariable(EnvFileConstants.ACCESS_TOKEN_LIFETIME_IN_DAYS), out tokenLifetime);
        var tokenDescriptor = new JwtSecurityToken(issuer, audience, claims,
            expires: DateTime.Now.AddDays(tokenLifetime),
            signingCredentials: credentials);
        string res = string.Empty;
        try
        {
            res = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
        catch (Exception ex)
        {

        }
        return res;
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token, string key)
    {
        ClaimsPrincipal principal = null;

        try
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            var check = jwtSecurityToken!.Header.Alg.Equals(
                            SecurityAlgorithms.HmacSha256Signature, StringComparison.InvariantCultureIgnoreCase);

            if (jwtSecurityToken == null || !check)
            {
                throw new SecurityTokenException("Invalid Token");
            }
        }
        catch
        {

        }

        return principal!;
    }

    public bool ValidateTokenWithExpiryTime(string key, string issuer, string audience, string token)
    {
        var mySecret = Encoding.UTF8.GetBytes(key);
        var mySecurityKey = new SymmetricSecurityKey(mySecret);
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            tokenHandler.ValidateToken(token,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateLifetime = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = mySecurityKey,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
        }
        catch
        {
            return false;
        }

        return true;
    }

    public bool ValidateTokenWithoutExpiryTime(string key, string issuer, string audience, string token)
    {
        var mySecret = Encoding.UTF8.GetBytes(key);
        var mySecurityKey = new SymmetricSecurityKey(mySecret);
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            tokenHandler.ValidateToken(token,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateLifetime = false,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = mySecurityKey
                }, out SecurityToken validatedToken);
        }
        catch
        {
            return false;
        }

        return true;
    }
}
