using ApiTest.Data;
using ApiTest.DTOS;
using ApiTest.Entities;
using Microsoft.AspNetCore.Authorization;
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
    [Route("[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return Ok(user);
        }
        [HttpPost]
        public IActionResult CreateUser(AppUser user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return Created("Users",user);
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
                return Ok();
            }
            else
                return NotFound();
        }
        [HttpPut]
        public IActionResult UpdateUser(AppUser user)
        {
            var existingUser = _context.Users.FirstOrDefault( u => u.Id == user.Id);
            if (existingUser != null)
            {
                existingUser.UserName = user.UserName;
                _context.SaveChanges();
                return Ok();
            }
            else
                return NotFound();

        }





    }
}
