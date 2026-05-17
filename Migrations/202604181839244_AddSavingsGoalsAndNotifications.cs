namespace SmartSpend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSavingsGoalsAndNotifications : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Title = c.String(nullable: false),
                        Message = c.String(nullable: false),
                        Type = c.String(),
                        Icon = c.String(),
                        IsRead = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.SavingsGoals",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        Icon = c.String(),
                        TargetAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SavedAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TargetDate = c.DateTime(),
                        IsCompleted = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SavingsGoals", "UserId", "dbo.Users");
            DropForeignKey("dbo.Notifications", "UserId", "dbo.Users");
            DropIndex("dbo.SavingsGoals", new[] { "UserId" });
            DropIndex("dbo.Notifications", new[] { "UserId" });
            DropTable("dbo.SavingsGoals");
            DropTable("dbo.Notifications");
        }
    }
}
