namespace Transparent.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddArgumentTableUserWeighting : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Arguments", "UserWeighting", c => c.Int(nullable: false));
            CreateIndex("dbo.Arguments", "UserWeighting");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Arguments", new[] { "UserWeighting" });
            DropColumn("dbo.Arguments", "UserWeighting");
        }
    }
}
