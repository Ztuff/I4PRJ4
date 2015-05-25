namespace SmartFridge_WebDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangednullableDateTimetoDateTimeinmodels : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ListItems", "ShelfLife", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ListItems", "ShelfLife", c => c.DateTime());
        }
    }
}
