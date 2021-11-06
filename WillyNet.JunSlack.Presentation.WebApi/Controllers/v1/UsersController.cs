using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WillyNet.JunSlack.Core.Application.Features.Users.Commands;
using WillyNet.JunSlack.Core.Application.Features.Users.Queries.GetAll;

namespace WillyNet.JunSlack.Presentation.WebApi.Controllers.v1
{
    public class UsersController : BaseApiController
    {
        [HttpPut("UpdateColors/{id}")]
        public async Task<IActionResult> UpdateColors(string id, [FromBody] EditUserCommand request)
        {
            request.UserId = id;
            return Ok(await Mediator.Send(request));
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(await Mediator.Send(new GetAllUsers()));
        }               
    }
}
