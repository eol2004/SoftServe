namespace VirtoCommerce.Platform.Data.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AccountAddPinCardCode : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PlatformAccount", new[] { "UserName" });
            AddColumn("dbo.PlatformAccount", "DisplayName", c => c.String(maxLength: 256));
            Sql("UPDATE PlatformAccount SET PlatformAccount.DisplayName = PlatformAccount.UserName");
            AddColumn("dbo.PlatformAccount", "PinCode", c => c.String(maxLength: 64));
            AddColumn("dbo.PlatformAccount", "CardCode", c => c.Long());
            AlterColumn("dbo.PlatformAccount", "UserName", c => c.String(maxLength: 128));
            AlterColumn("dbo.PlatformAccount", "DisplayName", c => c.String(nullable: false, maxLength: 256));
            CreateIndex("dbo.PlatformAccount", "UserName");
            CreateIndex("dbo.PlatformAccount", "DisplayName");
            CreateIndex("dbo.PlatformAccount", "PinCode");
            CreateIndex("dbo.PlatformAccount", "CardCode");
        }
        
        public override void Down()
        {
            DropIndex("dbo.PlatformAccount", new[] { "CardCode" });
            DropIndex("dbo.PlatformAccount", new[] { "PinCode" });
            DropIndex("dbo.PlatformAccount", new[] { "DisplayName" });
            DropIndex("dbo.PlatformAccount", new[] { "UserName" });
            AlterColumn("dbo.PlatformAccount", "UserName", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.PlatformAccount", "CardCode");
            DropColumn("dbo.PlatformAccount", "PinCode");
            DropColumn("dbo.PlatformAccount", "DisplayName");
            CreateIndex("dbo.PlatformAccount", "UserName");
        }
    }
}
