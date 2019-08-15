namespace VirtoCommerce.Platform.Data.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SettingAddCompany : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PlatformSetting", "CompanyId", c => c.String(maxLength: 128));
            CreateIndex("dbo.PlatformSetting", "CompanyId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.PlatformSetting", new[] { "CompanyId" });
            DropColumn("dbo.PlatformSetting", "CompanyId");
        }
    }
}
