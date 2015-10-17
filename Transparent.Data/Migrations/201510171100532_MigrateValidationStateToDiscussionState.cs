namespace Transparent.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrateValidationStateToDiscussionState : DbMigration
    {
        public override void Up()
        {
            Sql("UPDATE [dbo].[Tickets] SET State = 20 WHERE State = 10");
        }
        
        public override void Down()
        {
        }
    }
}
