namespace GroupProject1025.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyDevTaskClass : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DevNotices", "Title", c => c.String());
            AddColumn("dbo.DevProjects", "ProjectDesc", c => c.String());
            AddColumn("dbo.DevTasks", "TaskTitle", c => c.String());
            AddColumn("dbo.DevTasks", "Content", c => c.String());
            DropColumn("dbo.DevTasks", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DevTasks", "Name", c => c.String());
            DropColumn("dbo.DevTasks", "Content");
            DropColumn("dbo.DevTasks", "TaskTitle");
            DropColumn("dbo.DevProjects", "ProjectDesc");
            DropColumn("dbo.DevNotices", "Title");
        }
    }
}
