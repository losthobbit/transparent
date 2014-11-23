namespace Transparent.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedTableTicketColumnCreatedDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tickets", "CreatedDate", c => c.DateTime(nullable: false));
            CreateIndex("dbo.Tickets", "CreatedDate", false, "CreatedDate_IX");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Tickets", "CreatedDate_IX");
            DropColumn("dbo.Tickets", "CreatedDate");
        }
    }
}
