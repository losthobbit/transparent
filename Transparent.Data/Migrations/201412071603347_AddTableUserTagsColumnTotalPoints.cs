namespace Transparent.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTableUserTagsColumnTotalPoints : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserTags", "TotalPoints", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserTags", "TotalPoints");
        }
    }
}
