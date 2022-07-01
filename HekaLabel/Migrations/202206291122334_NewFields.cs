namespace HekaLabel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Category", "RevisionNo", c => c.String(maxLength: 4000));
            AddColumn("dbo.Category", "DeviceNo", c => c.String(maxLength: 4000));
            AddColumn("dbo.Category", "FirmNo", c => c.String(maxLength: 4000));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Category", "FirmNo");
            DropColumn("dbo.Category", "DeviceNo");
            DropColumn("dbo.Category", "RevisionNo");
        }
    }
}
