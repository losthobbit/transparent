namespace Transparent.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTicketTableModifiedDateColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tickets", "ModifiedDate", c => c.DateTime(nullable: false));
            CreateIndex("dbo.Tickets", "ModifiedDate");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Tickets", new[] { "ModifiedDate" });
            DropColumn("dbo.Tickets", "ModifiedDate");
        }
    }
}
