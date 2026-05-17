namespace SmartSpend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSubscriptions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Subscriptions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ServiceName = c.String(nullable: false),
                        CustomName = c.String(),
                        Icon = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NextBillingDate = c.DateTime(nullable: false),
                        Frequency = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        WantToCancel = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Subscriptions", "UserId", "dbo.Users");
            DropIndex("dbo.Subscriptions", new[] { "UserId" });
            DropTable("dbo.Subscriptions");
        }
    }
}
