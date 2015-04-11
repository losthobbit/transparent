namespace Transparent.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserPointsFkTicketId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserPoints", "FkTicketId", c => c.Int());
            CreateIndex("dbo.UserPoints", "FkTicketId");
            AddForeignKey("dbo.UserPoints", "FkTicketId", "dbo.Tickets", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserPoints", "FkTicketId", "dbo.Tickets");
            DropIndex("dbo.UserPoints", new[] { "FkTicketId" });
            DropColumn("dbo.UserPoints", "FkTicketId");
        }
    }
}
