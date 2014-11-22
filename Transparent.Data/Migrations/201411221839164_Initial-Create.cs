namespace Transparent.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserProfile",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(nullable: false, maxLength: 100),
                        Email = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.UserId)
                .Index(t => t.UserName, unique:true)
                .Index(t => t.Email, unique:true);
            
            CreateTable(
                "dbo.Points",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FkUserId = c.Int(nullable: false),
                        FkPointType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfile", t => t.FkUserId, cascadeDelete: true)
                .ForeignKey("dbo.PointTypes", t => t.FkPointType, cascadeDelete: true)
                .Index(t => t.FkUserId)
                .Index(t => t.FkPointType);
            
            CreateTable(
                "dbo.PointTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Tickets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FkUserId = c.Int(nullable: false),
                        Heading = c.String(nullable: false, maxLength: 100),
                        Body = c.String(nullable: false, maxLength: 10000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfile", t => t.FkUserId, cascadeDelete: true)
                .Index(t => t.FkUserId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Tickets", new[] { "FkUserId" });
            DropIndex("dbo.Points", new[] { "FkPointType" });
            DropIndex("dbo.Points", new[] { "FkUserId" });
            DropForeignKey("dbo.Tickets", "FkUserId", "dbo.UserProfile");
            DropForeignKey("dbo.Points", "FkPointType", "dbo.PointTypes");
            DropForeignKey("dbo.Points", "FkUserId", "dbo.UserProfile");
            DropTable("dbo.Tickets");
            DropTable("dbo.PointTypes");
            DropTable("dbo.Points");
            DropTable("dbo.UserProfile");
        }
    }
}
