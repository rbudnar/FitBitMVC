namespace FitBitMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FitbitUsers",
                c => new
                    {
                        UniqueID = c.Int(nullable: false, identity: true),
                        FitbitID = c.String(),
                        AccessToken = c.String(),
                        RefreshToken = c.String(),
                        Email = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        TimeStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UniqueID);
            
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Enabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.GroupFitbitUsers",
                c => new
                    {
                        Group_ID = c.Int(nullable: false),
                        FitbitUser_UniqueID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Group_ID, t.FitbitUser_UniqueID })
                .ForeignKey("dbo.Groups", t => t.Group_ID, cascadeDelete: true)
                .ForeignKey("dbo.FitbitUsers", t => t.FitbitUser_UniqueID, cascadeDelete: true)
                .Index(t => t.Group_ID)
                .Index(t => t.FitbitUser_UniqueID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GroupFitbitUsers", "FitbitUser_UniqueID", "dbo.FitbitUsers");
            DropForeignKey("dbo.GroupFitbitUsers", "Group_ID", "dbo.Groups");
            DropIndex("dbo.GroupFitbitUsers", new[] { "FitbitUser_UniqueID" });
            DropIndex("dbo.GroupFitbitUsers", new[] { "Group_ID" });
            DropTable("dbo.GroupFitbitUsers");
            DropTable("dbo.Groups");
            DropTable("dbo.FitbitUsers");
        }
    }
}
