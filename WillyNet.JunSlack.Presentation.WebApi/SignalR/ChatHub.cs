using MediatR;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using WillyNet.JunSlack.Core.Application.Features.Messages.Commands;
using WillyNet.JunSlack.Core.Application.Features.TypingNotifications.Commands;

namespace WillyNet.JunSlack.Presentation.WebApi.SignalR
{
    public class ChatHub : Hub
    {
        private readonly IMediator _mediator;
        public ChatHub(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task SendMessage(CreateMessageCommand command)
        {
            var message = await _mediator.Send(command);

            await Clients.All.SendAsync("ReciveMessage", message);
        }
        public async Task SendTypingNotification(CreateTypingNotificationCommand command)
        {
            var typing = await _mediator.Send(command);

            await Clients.All.SendAsync("ReceiveTypingNotification", typing);
        }

        public async Task DeleteTypingNotification()
        {
            var typing =
                await _mediator
                    .Send(new DeleteTypingNotificationCommand());

            await Clients
                .All
                .SendAsync("ReceiveDeleteTypingNotification", typing);
        }
    }
}
