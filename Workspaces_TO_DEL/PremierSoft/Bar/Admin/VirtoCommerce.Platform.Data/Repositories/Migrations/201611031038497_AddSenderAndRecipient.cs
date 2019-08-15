namespace VirtoCommerce.Platform.Data.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSenderAndRecipient : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PlatformNotificationTemplate", "Sender", c => c.String());
            AddColumn("dbo.PlatformNotificationTemplate", "Recipient", c => c.String());
            AlterColumn("dbo.PlatformSetting", "Description", c => c.String(maxLength: 1024));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PlatformSetting", "Description", c => c.String(maxLength: 256));
            DropColumn("dbo.PlatformNotificationTemplate", "Recipient");
            DropColumn("dbo.PlatformNotificationTemplate", "Sender");
        }
    }
}
