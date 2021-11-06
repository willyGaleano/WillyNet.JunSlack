using Ardalis.Specification.EntityFrameworkCore;
using WillyNet.JunSlack.Core.Application.Interface.Repositories;
using WillyNet.JunSlack.Infraestructure.Persistence.Contexts;

namespace WillyNet.JunSlack.Infraestructure.Persistence.Repositories
{
    public class RepositoryGenericSpecification<T> : RepositoryBase<T>, IRepositoryGenericSpecification<T> where T : class
    {
        private readonly ApplicationDbContext _dbContext;
        public RepositoryGenericSpecification(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
