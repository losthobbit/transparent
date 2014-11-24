namespace Transparent.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateTableTicketUserRank : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TicketUserRanks",
                c => new
                    {
                        FkTicketId = c.Int(nullable: false),
                        FkUserId = c.Int(nullable: false),
                        Up = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.FkTicketId, t.FkUserId })
                .ForeignKey("dbo.Tickets", t => t.FkTicketId, cascadeDelete: false)
                .ForeignKey("dbo.UserProfile", t => t.FkUserId, cascadeDelete: true)
                .Index(t => t.FkTicketId)
                .Index(t => t.FkUserId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.TicketUserRanks", new[] { "FkUserId" });
            DropIndex("dbo.TicketUserRanks", new[] { "FkTicketId" });
            DropForeignKey("dbo.TicketUserRanks", "FkUserId", "dbo.UserProfile");
            DropForeignKey("dbo.TicketUserRanks", "FkTicketId", "dbo.Tickets");
            DropTable("dbo.TicketUserRanks");
        }
    }
}
