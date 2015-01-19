namespace Transparent.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedTestMarkingInfoToUserPoint : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TestMarkings",
                c => new
                    {
                        FkUserPointId = c.Int(nullable: false),
                        FkUserId = c.Int(nullable: false),
                        Passed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.FkUserPointId, t.FkUserId })
                .ForeignKey("dbo.UserPoints", t => t.FkUserPointId, cascadeDelete: true)
                .ForeignKey("dbo.UserProfile", t => t.FkUserId)
                .Index(t => t.FkUserPointId)
                .Index(t => t.FkUserId);
            
            AddColumn("dbo.UserPoints", "MarkingComplete", c => c.Boolean(nullable: false));
            AlterColumn("dbo.UserPoints", "Answer", c => c.String());
            CreateIndex("dbo.UserPoints", "MarkingComplete");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TestMarkings", "FkUserId", "dbo.UserProfile");
            DropForeignKey("dbo.TestMarkings", "FkUserPointId", "dbo.UserPoints");
            DropIndex("dbo.UserPoints", new[] { "MarkingComplete" });
            DropIndex("dbo.TestMarkings", new[] { "FkUserId" });
            DropIndex("dbo.TestMarkings", new[] { "FkUserPointId" });
            AlterColumn("dbo.UserPoints", "Answer", c => c.String(maxLength: 2000));
            DropColumn("dbo.UserPoints", "MarkingComplete");
            DropTable("dbo.TestMarkings");
        }
    }
}
