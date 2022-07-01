namespace HekaLabel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Category",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 4000),
                        ModelNo = c.String(maxLength: 4000),
                        ModelName = c.String(maxLength: 4000),
                        ShiftCode = c.String(maxLength: 4000),
                        PrintDate = c.Boolean(nullable: false),
                        PrintShift = c.Boolean(nullable: false),
                        PrintBarcode = c.Boolean(nullable: false),
                        SerialNo = c.Int(nullable: false),
                        PrintCount = c.Int(nullable: false),
                        CopyCount = c.Int(nullable: false),
                        XModelNo = c.Int(),
                        YModelNo = c.Int(),
                        XModelName = c.Int(),
                        YModelName = c.Int(),
                        XShiftCode = c.Int(),
                        YShiftCode = c.Int(),
                        XDate = c.Int(),
                        YDate = c.Int(),
                        XBarcode = c.Int(),
                        YBarcode = c.Int(),
                        BarcodeSize = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Category");
        }
    }
}
