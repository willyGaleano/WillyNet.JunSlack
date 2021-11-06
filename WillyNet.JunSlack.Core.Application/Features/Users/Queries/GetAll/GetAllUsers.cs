using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WillyNet.JunSlack.Core.Application.DTOs;
using WillyNet.JunSlack.Core.Application.Interface.Repositories;
using WillyNet.JunSlack.Core.Application.Wrappers;
using WillyNet.JunSlack.Core.Domain.Entities;

namespace WillyNet.JunSlack.Core.Application.Features.Users.Queries.GetAll
{
    public class GetAllUsers : IRequest<Response<IEnumerable<UserDto>>>
    {
    }

    public class GetAllUsersHandler : IRequestHandler<GetAllUsers, Response<IEnumerable<UserDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryGenericSpecification<AppUser> _repositoryAsync;

        public GetAllUsersHandler(IRepositoryGenericSpecification<AppUser> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<IEnumerable<UserDto>>> Handle(GetAllUsers request, CancellationToken cancellationToken)
        {
            var users = await _repositoryAsync.ListAsync(cancellationToken);

            var usersDto = _mapper.Map<IEnumerable<UserDto>>(users);

            return new Response<IEnumerable<UserDto>>(usersDto, "Contulta exitosa.");
        }
    }
}
