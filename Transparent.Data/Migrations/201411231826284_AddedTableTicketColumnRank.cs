namespace Transparent.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedTableTicketColumnRank : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tickets", "Rank", c => c.Int(nullable: false));
            CreateIndex("dbo.Tickets", "Rank", false, "Rank_IX");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Tickets", "Rank_IX");
            DropColumn("dbo.Tickets", "Rank");
        }
    }
}
