using System;
using dotNetAPi.Dtos;
using dotNetAPi.Enum;
using dotNetAPi.Models;
using dotNetAPi.RBAC;
using dotNetAPi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Action = dotNetAPi.Enum.Action;

namespace dotNetAPi.Controllers
{
	[ApiController] //This tells ASP.NET Core:This class is an API controller.
	[Route("api/[controller]")] //This defines the base URL for this controller.[controller] is replaced by controller name without Controller. api/user

    public class UserController : ControllerBase  //This inherits built-in API features.ControllerBase gives:Ok() BadRequest() NotFound() request/response handling
    {
		private readonly UserService UserService;
		public UserController(UserService userService)
		{
			UserService = userService;
		}

		[HttpPost("register")]
		public async Task<IActionResult> UserRegistration([FromBody] Iuser dto)
		{
			var result = await UserService.RegisterUser(dto);


			if(result == null)
			{
				return BadRequest("User is not created...");
			}

			return Ok(new { message = "User Registered" ,data = result });	
		}


		[HttpPost("login")]
		public async Task<IActionResult> login([FromBody] Ilogin ilogin)
		{
			
			var login = await UserService.loginUser(ilogin);


			if(login == null)
			{
				return BadRequest("Email or Password Invalid...");
			}

		

			return Ok(new
			{
				message = "user login success",
				data = login,
				
			});

		}

		[Authorize]
		[Permission(Feature.Home , Action.Read)]
		[HttpGet("getAllUsers")]
		public async Task<IActionResult> getAllUsers()
		{
			var getAllUsers = await UserService.getAllUsers();

			return Ok(new { message = "user fetched success", data = getAllUsers });
		}

		[HttpGet("getUserById/{id}")]
		public async Task<IActionResult> getUserById(int id)
		{
			var getUserById = await UserService.getUserById(id);

			if(getUserById == null)
			{
				return BadRequest("User Not Exist...");

			}

			return Ok(new
			{
				message = "user fetched success",
				data = getUserById
			});
		}

		[HttpPut("updateUserById/{id}")]
		public async Task<IActionResult> updateUserById(int id, [FromBody] Iuser dto) 
		{
			var update = await UserService.updateByUserId(id, dto);

			if(update == null)
			{
				return BadRequest("user not found");
			}

			return Ok(new
			{
				message = "user updated success",
				data = update
			});
		}
	}
}

