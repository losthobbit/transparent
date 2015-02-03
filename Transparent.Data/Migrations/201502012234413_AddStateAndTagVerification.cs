namespace Transparent.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStateAndTagVerification : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tickets", "State", c => c.Int(nullable: false));
            AddColumn("dbo.TicketTags", "FkCreatedById", c => c.Int());
            AddColumn("dbo.TicketTags", "Verified", c => c.Boolean(nullable: false));
            CreateIndex("dbo.Tickets", "State");
            CreateIndex("dbo.TicketTags", "FkCreatedById");
            AddForeignKey("dbo.TicketTags", "FkCreatedById", "dbo.UserProfile", "UserId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TicketTags", "FkCreatedById", "dbo.UserProfile");
            DropIndex("dbo.TicketTags", new[] { "FkCreatedById" });
            DropIndex("dbo.Tickets", new[] { "State" });
            DropColumn("dbo.TicketTags", "Verified");
            DropColumn("dbo.TicketTags", "FkCreatedById");
            DropColumn("dbo.Tickets", "State");
        }
    }
}
