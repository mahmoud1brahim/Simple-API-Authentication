using ApiTest.Data;
using ApiTest.DTOS;
using ApiTest.Entities;
using ApiTest.Interfaces;
using ApiTest.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ApiTest.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;
        public AccountController(ApplicationDbContext context , ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto user)
        {
            bool isExist = await _context.Users.AnyAsync(x => x.UserName == user.Username);
            if (!isExist)
            {
                return Unauthorized("Username doesn't exist");
            }

            var appUser = await _context.Users.FirstOrDefaultAsync(x => x.UserName == user.Username);
            using var hmac = new HMACSHA512(appUser.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(user.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != appUser.PasswordHash[i])
                {
                    return Unauthorized("Invalid Password");
                }
            }
            return Ok(new UserDto
            {
                Username = appUser.UserName,
                Token = _tokenService.CreateToken(appUser)
            });

        }
        [HttpPost]
        public async Task<ActionResult> Register(RegisterDto user)
        {
            bool isExist = _context.Users.Any(x => x.UserName == user.Username);
            if (!isExist)
            {
                using var hmac = new HMACSHA512();

                AppUser appUser = new AppUser
                {
                    UserName = user.Username,
                    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(user.Password)),
                    PasswordSalt = hmac.Key
                };
                await _context.Users.AddAsync(appUser);
                await _context.SaveChangesAsync();
                return Created("User", appUser);
            }
            return BadRequest();

        }

    }
}
