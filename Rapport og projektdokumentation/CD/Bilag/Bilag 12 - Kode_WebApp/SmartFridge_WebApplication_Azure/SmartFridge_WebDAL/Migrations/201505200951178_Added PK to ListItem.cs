namespace SmartFridge_WebDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPKtoListItem : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.ListItems");
            AlterColumn("dbo.ListItems", "ShelfLife", c => c.DateTime(nullable: false));
            AddPrimaryKey("dbo.ListItems", new[] { "ListId", "ItemId", "Amount", "Volume", "ShelfLife" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.ListItems");
            AlterColumn("dbo.ListItems", "ShelfLife", c => c.DateTime());
            AddPrimaryKey("dbo.ListItems", new[] { "ListId", "ItemId" });
        }
    }
}
