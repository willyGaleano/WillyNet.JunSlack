using Ardalis.Specification;

namespace WillyNet.JunSlack.Core.Application.Interface.Repositories
{
    public interface IRepositoryGenericSpecification<T> : IRepositoryBase<T> where T : class { }
    public interface IReadRepositoryGenericSpecification<T> : IReadRepositoryBase<T> where T : class { }

}
