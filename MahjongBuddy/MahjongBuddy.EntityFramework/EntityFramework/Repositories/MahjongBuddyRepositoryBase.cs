using Abp.Domain.Entities;
using Abp.EntityFramework;
using Abp.EntityFramework.Repositories;

namespace MahjongBuddy.EntityFramework.Repositories
{
    public abstract class MahjongBuddyRepositoryBase<TEntity, TPrimaryKey> : EfRepositoryBase<MahjongBuddyDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected MahjongBuddyRepositoryBase(IDbContextProvider<MahjongBuddyDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //add common methods for all repositories
    }

    public abstract class MahjongBuddyRepositoryBase<TEntity> : MahjongBuddyRepositoryBase<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        protected MahjongBuddyRepositoryBase(IDbContextProvider<MahjongBuddyDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //do not add any method here, add to the class above (since this inherits it)
    }
}
