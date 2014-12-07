namespace Transparent.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTableUserTags : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserTags",
                c => new
                    {
                        FkUserId = c.Int(nullable: false),
                        FkTagId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FkUserId, t.FkTagId })
                .ForeignKey("dbo.UserProfile", t => t.FkUserId, cascadeDelete: true)
                .ForeignKey("dbo.Tags", t => t.FkTagId, cascadeDelete: true)
                .Index(t => t.FkUserId)
                .Index(t => t.FkTagId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.UserTags", new[] { "FkTagId" });
            DropIndex("dbo.UserTags", new[] { "FkUserId" });
            DropForeignKey("dbo.UserTags", "FkTagId", "dbo.Tags");
            DropForeignKey("dbo.UserTags", "FkUserId", "dbo.UserProfile");
            DropTable("dbo.UserTags");
        }
    }
}
