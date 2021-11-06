using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WillyNet.JunSlack.Core.Application.Features.Channels.Commands;
using WillyNet.JunSlack.Core.Application.Features.Channels.Queries;
using WillyNet.JunSlack.Core.Application.Features.Channels.Queries.GetAll;

namespace WillyNet.JunSlack.Presentation.WebApi.Controllers.v1
{
    public class ChannelController : BaseApiController
    {
        [HttpGet("GetAllChannels")]
        public async Task<IActionResult> GetAllChannels([FromQuery] GetAllChannel query)
        {
            var channels = await Mediator.Send(query);
            return Ok(channels);
        }

        [HttpGet("GetChannel/{id}")]
        public async Task<IActionResult> GetChannel(Guid id)
        {
            var channel = await Mediator.Send(new GetChannel { ChannelId = id });
            return Ok(channel);
        }

        [HttpPost("CreateChannel")]
        public async Task<IActionResult> CreateChannel(CreateChannelCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpGet("PrivateChannel/{id}")]
        public async Task<IActionResult> PrivateChannel(string id)
        {
            return Ok(await Mediator.Send(new GetPrivateChannel { UserId = id }));
        }

        [HttpPut("UpdateChannel/{id}")]
        public async Task<IActionResult> UpdateChannel(Guid id, [FromBody] EditChannelCommand command)
        {
            command.ChannelId = id;
            return Ok(await Mediator.Send(command));
        }
    }
}
