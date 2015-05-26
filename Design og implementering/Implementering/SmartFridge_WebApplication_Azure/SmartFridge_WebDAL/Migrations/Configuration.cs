using System.Data.Entity.Migrations;
using SmartFridge_WebDAL.Context;

namespace SmartFridge_WebDAL.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<SFContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(SFContext context)
        {

        }
    }
}
