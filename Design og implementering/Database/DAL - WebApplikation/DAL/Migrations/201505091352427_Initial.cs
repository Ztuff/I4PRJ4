namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Items",
                c => new
                    {
                        ItemId = c.Int(nullable: false, identity: true),
                        ItemName = c.String(),
                        StdVolume = c.Int(nullable: false),
                        StdUnit = c.String(),
                    })
                .PrimaryKey(t => t.ItemId);
            
            CreateTable(
                "dbo.ListItems",
                c => new
                    {
                        ListId = c.Int(nullable: false),
                        ItemId = c.Int(nullable: false),
                        Amount = c.Int(nullable: false),
                        Volume = c.Int(nullable: false),
                        Unit = c.String(),
                        ShelfLife = c.DateTime(),
                    })
                .PrimaryKey(t => new { t.ListId, t.ItemId })
                .ForeignKey("dbo.Items", t => t.ItemId, cascadeDelete: true)
                .ForeignKey("dbo.Lists", t => t.ListId, cascadeDelete: true)
                .Index(t => t.ListId)
                .Index(t => t.ItemId);
            
            CreateTable(
                "dbo.Lists",
                c => new
                    {
                        ListId = c.Int(nullable: false, identity: true),
                        ListName = c.String(),
                    })
                .PrimaryKey(t => t.ListId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ListItems", "ListId", "dbo.Lists");
            DropForeignKey("dbo.ListItems", "ItemId", "dbo.Items");
            DropIndex("dbo.ListItems", new[] { "ItemId" });
            DropIndex("dbo.ListItems", new[] { "ListId" });
            DropTable("dbo.Lists");
            DropTable("dbo.ListItems");
            DropTable("dbo.Items");
        }
    }
}
