namespace GroupProject1025.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyDevProjectClass : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DevProjects", "ProjectName", c => c.String());
            DropColumn("dbo.DevProjects", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DevProjects", "Name", c => c.Int(nullable: false));
            DropColumn("dbo.DevProjects", "ProjectName");
        }
    }
}
