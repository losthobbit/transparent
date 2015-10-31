namespace Transparent.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTicketTagTableTotalVotesColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TicketTags", "TotalPoints", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TicketTags", "TotalPoints");
        }
    }
}
