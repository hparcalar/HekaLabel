namespace HekaLabel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SpecialCode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Category", "SpecialCode", c => c.String(maxLength: 4000));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Category", "SpecialCode");
        }
    }
}
