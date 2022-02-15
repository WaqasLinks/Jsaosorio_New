namespace LeaveON.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SMTP1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "SmtpPort", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "SmtpPort", c => c.Int(nullable: false));
        }
    }
}
