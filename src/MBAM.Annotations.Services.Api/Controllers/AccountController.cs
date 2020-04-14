using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using MBAM.Annotations.Infra.CrossCutting.Identity.Models;
using MBAM.Annotations.Domain.Interfaces;
using MBAM.Annotations.Infra.CrossCutting.Identity.Authorization;
using MBAM.Annotations.Domain.Core.Notifications;
using MBAM.Annotations.Infra.CrossCutting.Identity.Models.AccountViewModels;

namespace MBAM.Annotations.Services.Api.Controllers
{
    public class AccountController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;
        private readonly IMediatorHandler _mediator;
        private readonly TokenDescriptor _tokenDescriptor;
        
        public AccountController(
                    UserManager<ApplicationUser> userManager,
                    SignInManager<ApplicationUser> signInManager,
                    ILoggerFactory loggerFactory,
                    TokenDescriptor tokenDescriptor,
                    INotificationHandler<DomainNotification> notifications,
                    IMediatorHandler mediator) : base(notifications, mediator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mediator = mediator;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _tokenDescriptor = tokenDescriptor;
        }

        private static long ToUnixEpochDate(DateTime date) => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

        [HttpPost]
        [AllowAnonymous]
        [Route("newAccount")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                NotifyInvalidErrorModel();
                return Response();
            }

            var user = new ApplicationUser { UserName = model.Name, Email = model.Email};

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddClaimAsync(user, new Claim("Annotations", "CanRead"));
                await _userManager.AddClaimAsync(user, new Claim("Annotations", "CanWrite"));

                if (!IsValidOperation())
                {
                    await _userManager.DeleteAsync(user);
                    return Response(model);
                }

                _logger.LogInformation(1, "User Created!");
                var response = GerarTokenUsuario(new LoginViewModel { Email = model.Email, Password = model.Password });
                return Response(response.Result);
            }
            AdicionarErrosIdentity(result);
            return Response(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                NotifyInvalidErrorModel();
                return Response(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                NotifyError(string.Empty, "User not founded");
                return Response(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user?.UserName, model.Password, false, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                var response = GerarTokenUsuario(model);
                return Response(response.Result);
            }

            NotifyError(result.ToString(), "Fail on login");
            return Response(model);
        }


        private async Task<object> GerarTokenUsuario(LoginViewModel login)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.NormalizedEmail == login.Email.Normalize().ToUpperInvariant());

            var userClaims = await _userManager.GetClaimsAsync(user);

            userClaims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            userClaims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            userClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            userClaims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));

            var identityClaims = new ClaimsIdentity();
            identityClaims.AddClaims(userClaims);

            var handler = new JwtSecurityTokenHandler();
            var signingConf = new SigningCredentialsConfiguration();
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _tokenDescriptor.Issuer,
                Audience = _tokenDescriptor.Audience,
                SigningCredentials = signingConf.SigningCredentials,
                Subject = identityClaims,
                NotBefore = DateTime.Now,
                Expires = DateTime.Now.AddMinutes(_tokenDescriptor.MinutesValid)
            });

            var encodedJwt = handler.WriteToken(securityToken);

            var response = new
            {
                access_token = encodedJwt,
                expires_in = DateTime.Now.AddMinutes(_tokenDescriptor.MinutesValid),
                user = new
                {
                    id = user.Id,
                    nome = user.UserName,
                    email = user.Email,
                    claims = userClaims.Select(c => new { c.Type, c.Value })
                }
            };

            return response;
        }



    }
}
