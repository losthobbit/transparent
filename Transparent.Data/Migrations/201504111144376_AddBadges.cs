namespace Transparent.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBadges : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserPoints", "FkTestId", "dbo.Tickets");
            DropIndex("dbo.UserPoints", new[] { "FkTestId" });
            AddColumn("dbo.UserProfile", "Badges", c => c.Int(nullable: false));
            AddColumn("dbo.UserPoints", "Badge", c => c.Int());
            AlterColumn("dbo.UserPoints", "FkTestId", c => c.Int());
            CreateIndex("dbo.UserPoints", "FkTestId");
            AddForeignKey("dbo.UserPoints", "FkTestId", "dbo.Tickets", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserPoints", "FkTestId", "dbo.Tickets");
            DropIndex("dbo.UserPoints", new[] { "FkTestId" });
            AlterColumn("dbo.UserPoints", "FkTestId", c => c.Int(nullable: false));
            DropColumn("dbo.UserPoints", "Badge");
            DropColumn("dbo.UserProfile", "Badges");
            CreateIndex("dbo.UserPoints", "FkTestId");
            AddForeignKey("dbo.UserPoints", "FkTestId", "dbo.Tickets", "Id", cascadeDelete: true);
        }
    }
}
