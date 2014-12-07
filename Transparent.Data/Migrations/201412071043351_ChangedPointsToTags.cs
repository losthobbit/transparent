namespace Transparent.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedPointsToTags : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Points", "FkUserId", "dbo.UserProfile");
            DropForeignKey("dbo.Points", "FkPointType", "dbo.PointTypes");
            DropIndex("dbo.Points", new[] { "FkUserId" });
            DropIndex("dbo.Points", new[] { "FkPointType" });
            CreateTable(
                "dbo.UserPoints",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FkUserId = c.Int(nullable: false),
                        FkTag = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfile", t => t.FkUserId, cascadeDelete: true)
                .ForeignKey("dbo.Tags", t => t.FkTag, cascadeDelete: true)
                .Index(t => t.FkUserId)
                .Index(t => t.FkTag);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 60),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name);
            
            DropTable("dbo.Points");
            DropTable("dbo.PointTypes");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.PointTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Points",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FkUserId = c.Int(nullable: false),
                        FkPointType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropIndex("dbo.UserPoints", new[] { "FkTag" });
            DropIndex("dbo.UserPoints", new[] { "FkUserId" });
            DropForeignKey("dbo.UserPoints", "FkTag", "dbo.Tags");
            DropForeignKey("dbo.UserPoints", "FkUserId", "dbo.UserProfile");
            DropTable("dbo.Tags");
            DropTable("dbo.UserPoints");
            CreateIndex("dbo.Points", "FkPointType");
            CreateIndex("dbo.Points", "FkUserId");
            AddForeignKey("dbo.Points", "FkPointType", "dbo.PointTypes", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Points", "FkUserId", "dbo.UserProfile", "UserId", cascadeDelete: true);
        }
    }
}
