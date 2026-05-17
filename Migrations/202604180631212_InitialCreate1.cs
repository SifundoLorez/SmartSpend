
namespace SmartSpend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Transactions", "UserId", "dbo.Users");
            CreateTable(
                "dbo.Budgets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        CategoryId = c.Int(nullable: false),
                        LimitAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Month = c.Int(nullable: false),
                        Year = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.CategoryId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        Icon = c.String(),
                        IsDefault = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId);
            
            AddColumn("dbo.Transactions", "CategoryId", c => c.Int(nullable: false));
            AddColumn("dbo.Transactions", "IsRecurring", c => c.Boolean(nullable: false));
            AddColumn("dbo.Transactions", "Frequency", c => c.String());
            CreateIndex("dbo.Transactions", "CategoryId");
            AddForeignKey("dbo.Transactions", "CategoryId", "dbo.Categories", "Id");
            AddForeignKey("dbo.Transactions", "UserId", "dbo.Users", "Id");
            DropColumn("dbo.Transactions", "Category");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Transactions", "Category", c => c.String(nullable: false));
            DropForeignKey("dbo.Transactions", "UserId", "dbo.Users");
            DropForeignKey("dbo.Transactions", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.Budgets", "UserId", "dbo.Users");
            DropForeignKey("dbo.Budgets", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.Categories", "UserId", "dbo.Users");
            DropIndex("dbo.Transactions", new[] { "CategoryId" });
            DropIndex("dbo.Categories", new[] { "UserId" });
            DropIndex("dbo.Budgets", new[] { "CategoryId" });
            DropIndex("dbo.Budgets", new[] { "UserId" });
            DropColumn("dbo.Transactions", "Frequency");
            DropColumn("dbo.Transactions", "IsRecurring");
            DropColumn("dbo.Transactions", "CategoryId");
            DropTable("dbo.Categories");
            DropTable("dbo.Budgets");
            AddForeignKey("dbo.Transactions", "UserId", "dbo.Users", "Id", cascadeDelete: true);
        }
    }
}
