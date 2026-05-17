namespace SmartSpend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Type = c.String(nullable: false),
                        Category = c.String(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false),
                        Surname = c.String(nullable: false),
                        IdNumber = c.String(nullable: false, maxLength: 13),
                        Age = c.Int(nullable: false),
                        Gender = c.String(),
                        Email = c.String(nullable: false),
                        PasswordHash = c.String(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "UserId", "dbo.Users");
            DropIndex("dbo.Transactions", new[] { "UserId" });
            DropTable("dbo.Users");
            DropTable("dbo.Transactions");
        }
    }
}
