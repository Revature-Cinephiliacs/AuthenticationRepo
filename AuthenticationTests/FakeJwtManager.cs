using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationTests
{
    public static class FakeJwtManager
    {
        public static string Issuer { get; } = Guid.NewGuid().ToString();
        public static string Audience { get; } = Guid.NewGuid().ToString();
        public static SecurityKey SecurityKey { get; }
        public static SigningCredentials SigningCredentials { get; }
        public static Claim[] claims {get;}

        private static readonly JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        private static readonly RandomNumberGenerator generator = RandomNumberGenerator.Create();
        private static readonly byte[] key = new byte[32];

        static FakeJwtManager()
        {
            generator.GetBytes(key);
            claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, "testuser"),
                new Claim(JwtRegisteredClaimNames.Email,"test@email.com"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            SecurityKey = new SymmetricSecurityKey(key) { KeyId = Guid.NewGuid().ToString() };
            SigningCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
        }

        /// <summary>
        /// Generates a mock JWT token
        /// </summary>
        /// <returns></returns>
        public static string GenerateJwtToken()
        {
            return tokenHandler.WriteToken(new JwtSecurityToken(Issuer, Audience, claims, null, DateTime.UtcNow.AddMinutes(10), SigningCredentials));
        }
    }
}