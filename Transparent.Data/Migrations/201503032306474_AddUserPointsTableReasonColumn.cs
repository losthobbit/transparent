namespace Transparent.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserPointsTableReasonColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserPoints", "Reason", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserPoints", "Reason");
        }
    }
}
