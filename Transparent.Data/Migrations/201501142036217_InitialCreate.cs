namespace Transparent.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            // Because the UserProfile table already exists, I don't create it, but update it instead:

            //CreateTable(
            //    "dbo.UserProfile",
            //    c => new
            //    {
            //        UserId = c.Int(nullable: false, identity: true),
            //        UserName = c.String(nullable: false, maxLength: 100),
            //        Email = c.String(nullable: false, maxLength: 100),
            //    })
            //    .PrimaryKey(t => t.UserId)
            //    .Index(t => t.UserName, unique: true)
            //    .Index(t => t.Email, unique: true);

            AddColumn("dbo.UserProfile", "Email", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.UserProfile", "UserName", c => c.String(nullable: false, maxLength: 100));
            CreateIndex("dbo.UserProfile", "UserName", unique: true);
            CreateIndex("dbo.UserProfile", "Email", unique: true);

            CreateTable(
                "dbo.Tickets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FkUserId = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        Rank = c.Int(nullable: false),
                        Heading = c.String(nullable: false, maxLength: 100),
                        Body = c.String(nullable: false),
                        TicketType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfile", t => t.FkUserId, cascadeDelete: true)
                .Index(t => t.FkUserId)
                .Index(t => t.CreatedDate)
                .Index(t => t.Rank);
            
            CreateTable(
                "dbo.TicketTags",
                c => new
                    {
                        FkTicketId = c.Int(nullable: false),
                        FkTagId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FkTicketId, t.FkTagId })
                .ForeignKey("dbo.Tags", t => t.FkTagId, cascadeDelete: true)
                .ForeignKey("dbo.Tickets", t => t.FkTicketId, cascadeDelete: true)
                .Index(t => t.FkTicketId)
                .Index(t => t.FkTagId);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 60),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
                       
            CreateTable(
                "dbo.UserTags",
                c => new
                    {
                        FkUserId = c.Int(nullable: false),
                        FkTagId = c.Int(nullable: false),
                        TotalPoints = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FkUserId, t.FkTagId })
                .ForeignKey("dbo.Tags", t => t.FkTagId, cascadeDelete: true)
                .ForeignKey("dbo.UserProfile", t => t.FkUserId, cascadeDelete: true)
                .Index(t => t.FkUserId)
                .Index(t => t.FkTagId);
            
            CreateTable(
                "dbo.TicketUserRanks",
                c => new
                    {
                        FkTicketId = c.Int(nullable: false),
                        FkUserId = c.Int(nullable: false),
                        Up = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.FkTicketId, t.FkUserId })
                .ForeignKey("dbo.Tickets", t => t.FkTicketId, cascadeDelete: true)
                .ForeignKey("dbo.UserProfile", t => t.FkUserId)
                .Index(t => t.FkTicketId)
                .Index(t => t.FkUserId);
            
            CreateTable(
                "dbo.UserPoints",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FkUserId = c.Int(nullable: false),
                        FkTagId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        FkTestId = c.Int(nullable: false),
                        Answer = c.String(maxLength: 2000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tags", t => t.FkTagId, cascadeDelete: true)
                .ForeignKey("dbo.Tickets", t => t.FkTestId, cascadeDelete: true)
                .ForeignKey("dbo.UserProfile", t => t.FkUserId)
                .Index(t => t.FkUserId)
                .Index(t => t.FkTagId)
                .Index(t => t.FkTestId);
            
            CreateTable(
                "dbo.TagRelationships",
                c => new
                    {
                        FkParentId = c.Int(nullable: false),
                        FkChildId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FkParentId, t.FkChildId })
                .ForeignKey("dbo.Tags", t => t.FkParentId)
                .ForeignKey("dbo.Tags", t => t.FkChildId)
                .Index(t => t.FkParentId)
                .Index(t => t.FkChildId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserPoints", "FkUserId", "dbo.UserProfile");
            DropForeignKey("dbo.UserPoints", "FkTestId", "dbo.Tickets");
            DropForeignKey("dbo.UserPoints", "FkTagId", "dbo.Tags");
            DropForeignKey("dbo.TicketTags", "FkTicketId", "dbo.Tickets");
            DropForeignKey("dbo.TicketUserRanks", "FkUserId", "dbo.UserProfile");
            DropForeignKey("dbo.TicketUserRanks", "FkTicketId", "dbo.Tickets");
            DropForeignKey("dbo.Tickets", "FkUserId", "dbo.UserProfile");
            DropForeignKey("dbo.UserTags", "FkUserId", "dbo.UserProfile");
            DropForeignKey("dbo.UserTags", "FkTagId", "dbo.Tags");
            DropForeignKey("dbo.TicketTags", "FkTagId", "dbo.Tags");
            DropForeignKey("dbo.TagRelationships", "FkChildId", "dbo.Tags");
            DropForeignKey("dbo.TagRelationships", "FkParentId", "dbo.Tags");
            DropIndex("dbo.TagRelationships", new[] { "FkChildId" });
            DropIndex("dbo.TagRelationships", new[] { "FkParentId" });
            DropIndex("dbo.UserPoints", new[] { "FkTestId" });
            DropIndex("dbo.UserPoints", new[] { "FkTagId" });
            DropIndex("dbo.UserPoints", new[] { "FkUserId" });
            DropIndex("dbo.TicketUserRanks", new[] { "FkUserId" });
            DropIndex("dbo.TicketUserRanks", new[] { "FkTicketId" });
            DropIndex("dbo.UserTags", new[] { "FkTagId" });
            DropIndex("dbo.UserTags", new[] { "FkUserId" });
            DropIndex("dbo.UserProfile", new[] { "Email" });
            DropIndex("dbo.UserProfile", new[] { "UserName" });
            DropIndex("dbo.Tags", new[] { "Name" });
            DropIndex("dbo.TicketTags", new[] { "FkTagId" });
            DropIndex("dbo.TicketTags", new[] { "FkTicketId" });
            DropIndex("dbo.Tickets", new[] { "Rank" });
            DropIndex("dbo.Tickets", new[] { "CreatedDate" });
            DropIndex("dbo.Tickets", new[] { "FkUserId" });
            DropTable("dbo.TagRelationships");
            DropTable("dbo.UserPoints");
            DropTable("dbo.TicketUserRanks");
            DropTable("dbo.UserTags");
            DropTable("dbo.Tags");
            DropTable("dbo.TicketTags");
            DropTable("dbo.Tickets");

            // Because the UserProfile table should still exist, I don't delete it, but update it instead:
            //DropTable("dbo.UserProfile");

            DropIndex("dbo.UserProfile", new[] { "Email" });
            DropIndex("dbo.UserProfile", new[] { "UserName" });
            AlterColumn("dbo.UserProfile", "UserName", c => c.String(nullable: false, maxLength: 56));
            DropColumn("dbo.UserProfile", "Email");
        }
    }
}
