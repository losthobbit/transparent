namespace Transparent.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddWeightedVoting : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TicketTagVotes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Points = c.Int(nullable: false),
                        FkUserId = c.Int(nullable: false),
                        TicketTag_FkTicketId = c.Int(),
                        TicketTag_FkTagId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfile", t => t.FkUserId, cascadeDelete: true)
                .ForeignKey("dbo.TicketTags", t => new { t.TicketTag_FkTicketId, t.TicketTag_FkTagId })
                .Index(t => t.FkUserId)
                .Index(t => new { t.TicketTag_FkTicketId, t.TicketTag_FkTagId });
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TicketTagVotes", new[] { "TicketTag_FkTicketId", "TicketTag_FkTagId" }, "dbo.TicketTags");
            DropForeignKey("dbo.TicketTagVotes", "FkUserId", "dbo.UserProfile");
            DropIndex("dbo.TicketTagVotes", new[] { "TicketTag_FkTicketId", "TicketTag_FkTagId" });
            DropIndex("dbo.TicketTagVotes", new[] { "FkUserId" });
            DropTable("dbo.TicketTagVotes");
        }
    }
}
