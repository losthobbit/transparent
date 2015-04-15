namespace Transparent.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTagsTableCompetentAndExpertPoints : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tags", "CompetentPoints", c => c.Int(nullable: false));
            AddColumn("dbo.Tags", "ExpertPoints", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tags", "ExpertPoints");
            DropColumn("dbo.Tags", "CompetentPoints");
        }
    }
}
