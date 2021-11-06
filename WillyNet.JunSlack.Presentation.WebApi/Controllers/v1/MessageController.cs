using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using WillyNet.JunSlack.Core.Application.Features.Messages.Commands;
using WillyNet.JunSlack.Presentation.WebApi.SignalR;

namespace WillyNet.JunSlack.Presentation.WebApi.Controllers.v1
{
    public class MessageController : BaseApiController
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public MessageController(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost("CreateMessage")]
        public async Task<IActionResult> CreateMessage([FromBody] CreateMessageCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPost("MediaUpload")]
        public async Task<IActionResult> MediaUpload([FromForm] CreateMessageCommand command)
        {
            var result = await Mediator.Send(command);

            await _hubContext.Clients.All.SendAsync("ReciveMessage", result);
            return Ok(result);
        }
    }
}
