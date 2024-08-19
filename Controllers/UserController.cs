using MedicalLabApi.Data;  
using MedicalLabApi.Models; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalLabApi.Controllers  
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;  

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        [HttpGet("users/details/{id}")]
        public async Task<ActionResult<User>> GetUserDetails(int id)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] User user)
        {
            Console.WriteLine($"Registering user with RoleId: {user.RoleId}");

            if (user.RoleId == 0)
            {
                user.RoleId = 2; 
            }
            else if (user.RoleId != 1 && user.RoleId != 2)
            {
                return BadRequest("RoleId must be either 1 or 2.");
            }

           
            var role = await _context.Roles.FindAsync(user.RoleId);
            if (role == null)
            {
                return BadRequest("Invalid role.");
            }

           
            _context.Attach(role);
            user.Role = role;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }



        [HttpPut("users/block/{id}")]
        public async Task<IActionResult> BlockUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.IsBlocked = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(int id, User updatedUser)
        {
            if (id != updatedUser.Id)
            {
                return BadRequest("User ID mismatch.");
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            user.Username = updatedUser.Username;
            user.Password = updatedUser.Password;
            user.Name = updatedUser.Name;
            user.DateOfBirth = updatedUser.DateOfBirth;
            user.ImageUrl = updatedUser.ImageUrl;
            user.RoleId = updatedUser.RoleId;
            user.IsBlocked = updatedUser.IsBlocked;
            user.LastLoginDate = updatedUser.LastLoginDate;
            user.Orders = updatedUser.Orders;

            _context.Users.Update(user); 
            await _context.SaveChangesAsync();

            return NoContent();
        }


        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
