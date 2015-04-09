namespace Transparent.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTicketHistoryTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TicketHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FkTicketId = c.Int(nullable: false),
                        FkUserId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tickets", t => t.FkTicketId, cascadeDelete: true)
                .ForeignKey("dbo.UserProfile", t => t.FkUserId, cascadeDelete: false)
                .Index(t => t.FkTicketId)
                .Index(t => t.FkUserId);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TicketHistories", "FkUserId", "dbo.UserProfile");
            DropForeignKey("dbo.TicketHistories", "FkTicketId", "dbo.Tickets");
            DropIndex("dbo.TicketHistories", new[] { "FkUserId" });
            DropIndex("dbo.TicketHistories", new[] { "FkTicketId" });
            DropTable("dbo.TicketHistories");
        }
    }
}
