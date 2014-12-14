namespace Transparent.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeTicketTypeNotNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tickets", "TicketType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tickets", "TicketType", c => c.Int());
        }
    }
}
