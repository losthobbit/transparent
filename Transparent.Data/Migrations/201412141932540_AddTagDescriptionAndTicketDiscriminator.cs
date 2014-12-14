namespace Transparent.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTagDescriptionAndTicketDiscriminator : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tags", "Description", c => c.String(nullable: false));
            AlterColumn("dbo.Tickets", "TicketType", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tickets", "TicketType", c => c.Int(nullable: false));
            DropColumn("dbo.Tags", "Description");
        }
    }
}
