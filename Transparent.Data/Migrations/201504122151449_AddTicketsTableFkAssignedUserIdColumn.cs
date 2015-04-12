namespace Transparent.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTicketsTableFkAssignedUserIdColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tickets", "FkAssignedUserId", c => c.Int());
            CreateIndex("dbo.Tickets", "FkAssignedUserId");
            AddForeignKey("dbo.Tickets", "FkAssignedUserId", "dbo.UserProfile", "UserId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tickets", "FkAssignedUserId", "dbo.UserProfile");
            DropIndex("dbo.Tickets", new[] { "FkAssignedUserId" });
            DropColumn("dbo.Tickets", "FkAssignedUserId");
        }
    }
}
