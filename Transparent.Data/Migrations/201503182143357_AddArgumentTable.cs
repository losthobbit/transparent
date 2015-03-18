namespace Transparent.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddArgumentTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Arguments",
                c => new
                    {
                        FkTicketId = c.Int(nullable: false),
                        FkUserId = c.Int(nullable: false),
                        Body = c.String(nullable: false),
                    })
                .PrimaryKey(t => new { t.FkTicketId, t.FkUserId })
                .ForeignKey("dbo.Tickets", t => t.FkTicketId, cascadeDelete: true)
                .ForeignKey("dbo.UserProfile", t => t.FkUserId, cascadeDelete: false)
                .Index(t => t.FkTicketId)
                .Index(t => t.FkUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Arguments", "FkUserId", "dbo.UserProfile");
            DropForeignKey("dbo.Arguments", "FkTicketId", "dbo.Tickets");
            DropIndex("dbo.Arguments", new[] { "FkUserId" });
            DropIndex("dbo.Arguments", new[] { "FkTicketId" });
            DropTable("dbo.Arguments");
        }
    }
}
