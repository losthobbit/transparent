namespace Transparent.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedContext : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TicketTags",
                c => new
                    {
                        FkTicketId = c.Int(nullable: false),
                        FkTagId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FkTicketId, t.FkTagId })
                .ForeignKey("dbo.Tickets", t => t.FkTicketId, cascadeDelete: true)
                .ForeignKey("dbo.Tags", t => t.FkTagId, cascadeDelete: true)
                .Index(t => t.FkTicketId)
                .Index(t => t.FkTagId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.TicketTags", new[] { "FkTagId" });
            DropIndex("dbo.TicketTags", new[] { "FkTicketId" });
            DropForeignKey("dbo.TicketTags", "FkTagId", "dbo.Tags");
            DropForeignKey("dbo.TicketTags", "FkTicketId", "dbo.Tickets");
            DropTable("dbo.TicketTags");
        }
    }
}
