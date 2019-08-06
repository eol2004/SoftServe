namespace VirtoCommerce.Platform.Data.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AccountAddCinemaServiceId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PlatformAccount", "CinemaServiceId", c => c.Guid());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PlatformAccount", "CinemaServiceId");
        }
    }
}
