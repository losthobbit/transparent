namespace Transparent.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserProfileFacebookIdColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfile", "FacebookId", c => c.String(maxLength: 16));
            Sql("CREATE UNIQUE INDEX IX_FacebookId ON dbo.UserProfile ( FacebookId ) " +
                "WHERE FacebookId IS NOT NULL");
        }
        
        public override void Down()
        {
            DropIndex("dbo.UserProfile", new[] { "FacebookId" });
            DropColumn("dbo.UserProfile", "FacebookId");
        }
    }
}
