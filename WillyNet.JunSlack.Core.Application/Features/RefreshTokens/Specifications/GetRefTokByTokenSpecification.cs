using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WillyNet.JunSlack.Core.Domain.Entities;

namespace WillyNet.JunSlack.Core.Application.Features.RefreshTokens.Specifications
{
    public class GetRefTokByTokenSpecification : Specification<RefreshToken>, ISingleResultSpecification
    {
        public GetRefTokByTokenSpecification(string tokenRefresh)
        {
            Query.Where(x => x.Token == tokenRefresh);
        }
    }
}
