namespace GroupProject1025.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSalaryToUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Salary", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "PMAllowance", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "PMAllowance");
            DropColumn("dbo.AspNetUsers", "Salary");
        }
    }
}
