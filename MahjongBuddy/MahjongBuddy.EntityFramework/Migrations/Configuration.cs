using System.Data.Entity.Migrations;
using MahjongBuddy.Migrations.SeedData;

namespace MahjongBuddy.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<MahjongBuddy.EntityFramework.MahjongBuddyDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "MahjongBuddy";
        }

        protected override void Seed(MahjongBuddy.EntityFramework.MahjongBuddyDbContext context)
        {
            new InitialDataBuilder(context).Build();
        }
    }
}
