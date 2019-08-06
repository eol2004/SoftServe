namespace VirtoCommerce.Platform.Data.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AccountAddCompany : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PlatformAccount", "CompanyId", c => c.String(maxLength: 128));
            DropColumn("dbo.PlatformAccount", "StoreId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PlatformAccount", "StoreId", c => c.String(maxLength: 128));
            DropColumn("dbo.PlatformAccount", "CompanyId");
        }
    }
}
