namespace VirtoCommerce.Platform.Data.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SettingIntCompany : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PlatformSetting", new[] { "CompanyId" });
            AlterColumn("dbo.PlatformSetting", "CompanyId", c => c.Int());
            AlterColumn("dbo.PlatformAccount", "CompanyId", c => c.Int());
            CreateIndex("dbo.PlatformSetting", "CompanyId");
            CreateIndex("dbo.PlatformAccount", "CompanyId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.PlatformAccount", new[] { "CompanyId" });
            DropIndex("dbo.PlatformSetting", new[] { "CompanyId" });
            AlterColumn("dbo.PlatformAccount", "CompanyId", c => c.String(maxLength: 128));
            AlterColumn("dbo.PlatformSetting", "CompanyId", c => c.String(maxLength: 128));
            CreateIndex("dbo.PlatformSetting", "CompanyId");
        }
    }
}
