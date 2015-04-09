namespace Transparent.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTicketHistoryTableStateColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TicketHistories", "State", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TicketHistories", "State");
        }
    }
}
