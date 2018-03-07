using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Microsoft.Extensions.Options;
using TravelActive.Common.Utilities;
using TravelActive.Data;
using TravelActive.Models.Entities;
using TravelActive.Models.ViewModels;

namespace TravelActive.Services
{
    public class TokenService : Service
    {
        private readonly JwtSettings jwtSettings;
        public TokenService(TravelActiveContext context, IOptions<JwtSettings> jwtSettings) : base(context)
        {
            this.jwtSettings = jwtSettings.Value;
        }

        public AccessTokenViewModel GetAccessTokenAsync(User user)
        {
            var userRolesId = Context.UserRoles.Where(x => x.UserId == user.Id);
            var userRoles = Context.Roles.Select(x => x.Name).ToArray();
            var payload = new Dictionary<string, object>
            {
                { Constants.Claims.Id, user.Id },
                { Constants.Claims.Username, user.UserName },
                { Constants.Claims.Email, user.Email },
                { Constants.Claims.EmailConfirmed, user.EmailConfirmed },
                { Constants.Claims.Roles, userRoles }
            };
            return GetToken(payload);
        }
        private AccessTokenViewModel GetToken(Dictionary<string, object> payload)
        {
            var secret = jwtSettings.SecretKey;
            payload.Add("iss", jwtSettings.Issuer);
            payload.Add("aud", jwtSettings.Audience);
            payload.Add("nbf", DateTimeOffset.Now.ToUnixTimeSeconds());
            payload.Add("iat", DateTimeOffset.Now.ToUnixTimeSeconds());
            var expires = DateTimeOffset.Now.AddDays(7);
            payload.Add(Constants.Claims.Exparation, expires.ToUnixTimeSeconds());
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            return new AccessTokenViewModel()
            {
                AccessToken = encoder.Encode(payload, secret),
                Expires = expires
            };
        }

        public async Task LogOut(string authorizationHeader)
        {
            string tokenString = authorizationHeader.Substring(7);
            JwtSecurityToken jwtToken = new JwtSecurityToken(tokenString);
            var claim = jwtToken.Claims.First(c => c.Type == Constants.Claims.Exparation);
            var userId = jwtToken.Claims.First(c => c.Type == Constants.Claims.Id).Value;
            long exparationDateUnix = long.Parse(claim.Value);
            DateTime exparationDate = UnixTimeStampToDateTime(exparationDateUnix);

            BlockedTokens blockedTokens = new BlockedTokens()
            {
                UserId = userId,
                Token = tokenString,
                ExparationDate = exparationDate
            };
            await Context.BlockedTokenses.AddAsync(blockedTokens);
            await Context.SaveChangesAsync();
        }
        private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);
            return dtDateTime;
        }
    }
}