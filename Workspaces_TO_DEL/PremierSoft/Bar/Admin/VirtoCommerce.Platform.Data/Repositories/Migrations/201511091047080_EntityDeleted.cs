namespace VirtoCommerce.Platform.Data.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EntityDeleted : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PlatformOperationLog", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.PlatformSetting", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.PlatformSettingValue", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.PlatformDynamicProperty", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.PlatformDynamicPropertyDictionaryItem", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.PlatformDynamicPropertyDictionaryItemName", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.PlatformDynamicPropertyObjectValue", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.PlatformDynamicPropertyName", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.PlatformAccount", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.PlatformApiAccount", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.PlatformRoleAssignment", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.PlatformRole", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.PlatformRolePermission", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.PlatformPermission", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.PlatformNotification", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.PlatformNotificationTemplate", "Deleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PlatformNotificationTemplate", "Deleted");
            DropColumn("dbo.PlatformNotification", "Deleted");
            DropColumn("dbo.PlatformPermission", "Deleted");
            DropColumn("dbo.PlatformRolePermission", "Deleted");
            DropColumn("dbo.PlatformRole", "Deleted");
            DropColumn("dbo.PlatformRoleAssignment", "Deleted");
            DropColumn("dbo.PlatformApiAccount", "Deleted");
            DropColumn("dbo.PlatformAccount", "Deleted");
            DropColumn("dbo.PlatformDynamicPropertyName", "Deleted");
            DropColumn("dbo.PlatformDynamicPropertyObjectValue", "Deleted");
            DropColumn("dbo.PlatformDynamicPropertyDictionaryItemName", "Deleted");
            DropColumn("dbo.PlatformDynamicPropertyDictionaryItem", "Deleted");
            DropColumn("dbo.PlatformDynamicProperty", "Deleted");
            DropColumn("dbo.PlatformSettingValue", "Deleted");
            DropColumn("dbo.PlatformSetting", "Deleted");
            DropColumn("dbo.PlatformOperationLog", "Deleted");
        }
    }
}
