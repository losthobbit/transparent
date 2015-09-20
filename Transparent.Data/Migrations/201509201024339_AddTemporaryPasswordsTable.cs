namespace Transparent.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTemporaryPasswordsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TemporaryPasswords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FkUserId = c.Int(nullable: false),
                        ExpiryDate = c.DateTime(nullable: false),
                        Hash = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfile", t => t.FkUserId, cascadeDelete: true)
                .Index(t => t.FkUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TemporaryPasswords", "FkUserId", "dbo.UserProfile");
            DropIndex("dbo.TemporaryPasswords", new[] { "FkUserId" });
            DropTable("dbo.TemporaryPasswords");
        }
    }
}
