namespace HekaLabel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CategoryPrinter_And_RevisionChangeLog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RevisionChangeLog",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CategoryId = c.Int(),
                        ModelNo = c.String(maxLength: 4000),
                        RevisionNo = c.String(maxLength: 4000),
                        ChangeDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Category", "LastPrinterName", c => c.String(maxLength: 4000));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Category", "LastPrinterName");
            DropTable("dbo.RevisionChangeLog");
        }
    }
}
