namespace Transparent.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Unknown : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TagRelationships",
                c => new
                    {
                        FkParentId = c.Int(nullable: false),
                        FkChildId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FkParentId, t.FkChildId })
                .ForeignKey("dbo.Tags", t => t.FkParentId, cascadeDelete: false)
                .ForeignKey("dbo.Tags", t => t.FkChildId, cascadeDelete: false)
                .Index(t => t.FkParentId)
                .Index(t => t.FkChildId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.TagRelationships", new[] { "FkChildId" });
            DropIndex("dbo.TagRelationships", new[] { "FkParentId" });
            DropForeignKey("dbo.TagRelationships", "FkChildId", "dbo.Tags");
            DropForeignKey("dbo.TagRelationships", "FkParentId", "dbo.Tags");
            DropTable("dbo.TagRelationships");
        }
    }
}
