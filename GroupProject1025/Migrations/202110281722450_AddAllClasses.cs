namespace GroupProject1025.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAllClasses : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DevNotices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ApplicationUserID = c.String(maxLength: 128),
                        MessageType = c.Int(nullable: false),
                        SenderId = c.String(),
                        Message = c.String(),
                        DevProjectId = c.Int(nullable: false),
                        DevTaskId = c.Int(nullable: false),
                        IsRead = c.Boolean(nullable: false),
                        NoticeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserID)
                .Index(t => t.ApplicationUserID);
            
            CreateTable(
                "dbo.DevProjects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.Int(nullable: false),
                        ApplicationUserID = c.String(maxLength: 128),
                        Budget = c.Int(nullable: false),
                        FinalCost = c.Int(nullable: false),
                        PlanStartDate = c.DateTime(nullable: false),
                        PlanEndDate = c.DateTime(nullable: false),
                        ActualStartDate = c.DateTime(nullable: false),
                        ActualEndDate = c.DateTime(nullable: false),
                        Deadline = c.DateTime(nullable: false),
                        Priority = c.Int(nullable: false),
                        IsNoticed = c.Boolean(nullable: false),
                        NoticeDate = c.DateTime(nullable: false),
                        NoticeTimes = c.Int(nullable: false),
                        IsCompleted = c.Boolean(nullable: false),
                        Percentage = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserID)
                .Index(t => t.ApplicationUserID);
            
            CreateTable(
                "dbo.DevTasks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        DevProjectId = c.Int(nullable: false),
                        ApplicationUserID = c.String(maxLength: 128),
                        PlanStartDate = c.DateTime(nullable: false),
                        PlanEndDate = c.DateTime(nullable: false),
                        ActualStartDate = c.DateTime(nullable: false),
                        ActualEndDate = c.DateTime(nullable: false),
                        Deadline = c.DateTime(nullable: false),
                        Comment = c.String(),
                        Percentage = c.Int(nullable: false),
                        IsCompleted = c.Boolean(nullable: false),
                        Priority = c.Int(nullable: false),
                        IsNoticed = c.Boolean(nullable: false),
                        NoticeDate = c.DateTime(nullable: false),
                        NoticeTimes = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserID)
                .Index(t => t.ApplicationUserID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DevTasks", "ApplicationUserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.DevProjects", "ApplicationUserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.DevNotices", "ApplicationUserID", "dbo.AspNetUsers");
            DropIndex("dbo.DevTasks", new[] { "ApplicationUserID" });
            DropIndex("dbo.DevProjects", new[] { "ApplicationUserID" });
            DropIndex("dbo.DevNotices", new[] { "ApplicationUserID" });
            DropTable("dbo.DevTasks");
            DropTable("dbo.DevProjects");
            DropTable("dbo.DevNotices");
        }
    }
}
