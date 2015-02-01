namespace Transparent.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserProfileTableVolunteerColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfile", "Services", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserProfile", "Services");
        }
    }
}
