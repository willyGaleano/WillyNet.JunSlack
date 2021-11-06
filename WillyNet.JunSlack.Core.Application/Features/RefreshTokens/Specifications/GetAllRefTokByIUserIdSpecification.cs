using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WillyNet.JunSlack.Core.Domain.Entities;

namespace WillyNet.JunSlack.Core.Application.Features.RefreshTokens.Specifications
{
    public class GetAllRefTokByIUserIdSpecification : Specification<RefreshToken>
    {
        public GetAllRefTokByIUserIdSpecification(string userAppId)
        {
            Query.Where(x => x.UserAppId == userAppId);
        }
    }
}
