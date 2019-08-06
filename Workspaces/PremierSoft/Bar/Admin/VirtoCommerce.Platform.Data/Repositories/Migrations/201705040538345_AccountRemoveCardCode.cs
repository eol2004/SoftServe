namespace VirtoCommerce.Platform.Data.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AccountRemoveCardCode : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PlatformAccount", new[] { "PinCode" });
            DropIndex("dbo.PlatformAccount", new[] { "CardCode" });
            DropColumn("dbo.PlatformAccount", "PinCode");
            DropColumn("dbo.PlatformAccount", "CardCode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PlatformAccount", "CardCode", c => c.Long());
            AddColumn("dbo.PlatformAccount", "PinCode", c => c.String(maxLength: 64));
            CreateIndex("dbo.PlatformAccount", "CardCode");
            CreateIndex("dbo.PlatformAccount", "PinCode");
        }
    }
}
