namespace Transparent.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserProfileLastActionDateColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfile", "LastActionDate", c => c.DateTime());
            CreateIndex("dbo.UserProfile", "LastActionDate");
        }
        
        public override void Down()
        {
            DropIndex("dbo.UserProfile", new[] { "LastActionDate" });
            DropColumn("dbo.UserProfile", "LastActionDate");
        }
    }
}
