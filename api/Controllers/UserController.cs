using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.User;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public UserController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll(){
            var users = _context.Users.Include(user => user.Pets).ToList().Select(users => users.ToDto());
            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult getById([FromRoute] int id){
            var user = _context.Users.Include(user => user.Pets).FirstOrDefault(u => u.Id == id);
            if(user == null){
                return NotFound();
            }
            return Ok(user.ToDto());
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateUserRequestDto userDto){
            var userModel = userDto.ToUserFromCreateDto();
            _context.Users.Add(userModel);
            _context.SaveChanges();
            return CreatedAtAction(nameof(getById), new { id = userModel.Id}, userModel.ToDto());
        }
        
        [HttpPut]
        [Route("{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] UpdateUserRequestDto userDto){
            var userModel = _context.Users.FirstOrDefault(user => user.Id == id);
            if (userModel == null){
                return NotFound();
            }
            userModel.Age = userDto.Age;
            userModel.FirstName = userDto.FirstName;
            userModel.LastName = userDto.LastName;

            _context.SaveChanges();

            return Ok(userModel.ToDto());
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult Delete([FromRoute] int id){
            var userModel = _context.Users.FirstOrDefault(user => user.Id == id);
            if (userModel == null){
                return NotFound();
            }
            _context.Users.Remove(userModel);

            _context.SaveChanges();

            return NoContent();
        }
        
    }
}