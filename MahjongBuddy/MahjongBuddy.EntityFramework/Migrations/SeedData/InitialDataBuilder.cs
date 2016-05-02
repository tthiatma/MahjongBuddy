using MahjongBuddy.EntityFramework;
using EntityFramework.DynamicFilters;

namespace MahjongBuddy.Migrations.SeedData
{
    public class InitialDataBuilder
    {
        private readonly MahjongBuddyDbContext _context;

        public InitialDataBuilder(MahjongBuddyDbContext context)
        {
            _context = context;
        }

        public void Build()
        {
            _context.DisableAllFilters();

            new DefaultEditionsBuilder(_context).Build();
            new DefaultTenantRoleAndUserBuilder(_context).Build();
            new DefaultLanguagesBuilder(_context).Build();
        }
    }
}
