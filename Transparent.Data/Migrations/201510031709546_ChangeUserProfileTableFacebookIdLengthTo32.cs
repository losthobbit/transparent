namespace Transparent.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeUserProfileTableFacebookIdLengthTo32 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.UserProfile", new[] { "FacebookId" });
            AlterColumn("dbo.UserProfile", "FacebookId", c => c.String(maxLength: 32));
            Sql("CREATE UNIQUE INDEX IX_FacebookId ON dbo.UserProfile ( FacebookId ) " +
                "WHERE FacebookId IS NOT NULL");
        }
        
        public override void Down()
        {
            DropIndex("dbo.UserProfile", new[] { "FacebookId" });
            AlterColumn("dbo.UserProfile", "FacebookId", c => c.String(maxLength: 16));
            CreateIndex("dbo.UserProfile", "FacebookId", unique: true);
        }
    }
}
