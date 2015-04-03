namespace Transparent.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTicketUserVotesTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TicketUserVotes",
                c => new
                    {
                        FkTicketId = c.Int(nullable: false),
                        FkUserId = c.Int(nullable: false),
                        For = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.FkTicketId, t.FkUserId })
                .ForeignKey("dbo.Tickets", t => t.FkTicketId, cascadeDelete: true)
                .ForeignKey("dbo.UserProfile", t => t.FkUserId, cascadeDelete: false)
                .Index(t => t.FkTicketId)
                .Index(t => t.FkUserId);
            
            AddColumn("dbo.Tickets", "VotesFor", c => c.Int(nullable: false));
            AddColumn("dbo.Tickets", "VotesAgainst", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TicketUserVotes", "FkUserId", "dbo.UserProfile");
            DropForeignKey("dbo.TicketUserVotes", "FkTicketId", "dbo.Tickets");
            DropIndex("dbo.TicketUserVotes", new[] { "FkUserId" });
            DropIndex("dbo.TicketUserVotes", new[] { "FkTicketId" });
            DropColumn("dbo.Tickets", "VotesAgainst");
            DropColumn("dbo.Tickets", "VotesFor");
            DropTable("dbo.TicketUserVotes");
        }
    }
}
