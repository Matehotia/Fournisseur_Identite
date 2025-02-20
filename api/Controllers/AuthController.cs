using Microsoft.AspNetCore.Mvc;
using App.Services;
using App.Models;

namespace App.Controllers;

[ApiController]
[Route("api")]
public class AuthController : Controller
{
    private readonly UserService service;
    private readonly RespGenerator resp;

    public AuthController(UserService sv, RespGenerator rp) {
        this.service = sv;
        this.resp = rp;
    }


    [HttpGet("users")]
    public IActionResult get_all_users() {
        try {
            return Ok(resp.generate(service.get_all_users(), null));
        }
        catch(Exception e) {
            return BadRequest(resp.generate(null, e.Message));
        }
        
    }


    [HttpPost("users")]
    public IActionResult sign_up([FromBody]User user) {
        try {
            this.service.sign_up(user);
            return Ok(resp.generate(new {
                message = "Check your email to validate your subscribing"
            }, null));
        } 
        catch(Exception e) {
            return BadRequest(resp.generate(null, e.Message));
        }
    }


    [HttpGet("reset/{user_id}")]
    public IActionResult reset_attempts([FromRoute] int user_id) {
        try {
            this.service.reset_attempt_for_user(user_id);
            return Ok(resp.generate(new 
            {
                message="Attempts reseted successfully !"
            }, null));
        }
        catch(Exception e) {
            return BadRequest(resp.generate(null, e.Message));
        }
    }


    [HttpPost("login/steps/1")]
    public IActionResult log_first([FromBody]LoginRequest1 logReq) {
        try {
            this.service.log_first_step(logReq);
            return Ok(resp.generate(new 
            {
                message="Done, check your email for the next step !"
            }, null));
        }
        catch(Exception e) {
            return BadRequest(resp.generate(null, e.Message));
        }
    }


    [HttpPost("login/steps/2/{user_id}")]
    public IActionResult log_second([FromRoute]int user_id, [FromBody]LoginRequest2 pin) {
        try {
            this.service.log_second_step(user_id, pin);
            return Ok(resp.generate(new 
            {
                message="Second step done, you are signed in, now check your email !"
            }, null));
        }
        catch(Exception e) {
            return BadRequest(resp.generate(null, e.Message));
        }
    }


    [HttpGet("users/{id}/profile")]
    public IActionResult showProfile([FromRoute]int id, [FromQuery]string token) {
        try {
            return Ok(
                resp.generate(this.service.get_profile(id, token), null)
            );
        }
        catch(Exception e) {
            return BadRequest(resp.generate(null, e.Message));
        }
    }


    [HttpPut("users/{id}")]
    public IActionResult modify_user([FromRoute]int id, [FromQuery]string token,
        [FromBody]User newUsr) {
        try {
            this.service.update_info(id, token, newUsr);
            return Ok(resp.generate(new 
            {
                message = "Your new information is set !"
            }, null));
        } 
        catch (Exception e) {
            return BadRequest(resp.generate(null, e.Message));
        }
    }


    // [HttpDelete("users/{id}")]
    // public IActionResult delete_user([FromRoute]int id, [FromQuery]string token) {
    //     try {
    //         this.service.delete_user(id, token);
    //         return Ok(resp.generate(new 
    //         {
    //             message = "Your is deleted successfully !"
    //         }, null));
    //     } 
    //     catch (Exception e) {
    //         return BadRequest(resp.generate(null, e.Message));
    //     }
    // }
}
