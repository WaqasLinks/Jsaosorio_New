namespace LeaveON.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SMTP : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "SmtpClient", c => c.String());
            AddColumn("dbo.AspNetUsers", "SmtpPassword", c => c.String());
            AddColumn("dbo.AspNetUsers", "SmtpPort", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "SmtpPort");
            DropColumn("dbo.AspNetUsers", "SmtpPassword");
            DropColumn("dbo.AspNetUsers", "SmtpClient");
        }
    }
}
