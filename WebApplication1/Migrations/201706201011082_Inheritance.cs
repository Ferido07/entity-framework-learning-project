namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Inheritance : DbMigration
    {
        public override void Up()
        {
            /*DropForeignKey("dbo.Enrollments", "StudentID", "dbo.Students");
            DropIndex("dbo.Enrollments", new[] { "StudentID" });
            CreateTable(
                "dbo.People",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        LastName = c.String(nullable: false, maxLength: 40),
                        FirstName = c.String(nullable: false, maxLength: 40),
                        HireDate = c.DateTime(),
                        EnrollmentDate = c.DateTime(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateIndex("dbo.Enrollments", "StudentID");
            AddForeignKey("dbo.Enrollments", "StudentID", "dbo.People", "ID", cascadeDelete: true);
            DropTable("dbo.Instructors");
            DropTable("dbo.Students");*/
           
            // Drop foreign keys and indexes that point to tables we're going to drop.
            DropForeignKey("dbo.Enrollments", "StudentID", "dbo.Students");
            DropIndex("dbo.Enrollments", new[] { "StudentID" });
            RenameTable(name: "dbo.Instructors", newName: "People");
            AddColumn("dbo.People", "EnrollmentDate", c => c.DateTime());
            AddColumn("dbo.People", "Discriminator", c => c.String(nullable: false, maxLength: 128, defaultValue:
        "Instructor"));
            AlterColumn("dbo.People", "HireDate", c => c.DateTime());
            AddColumn("dbo.People", "OldId", c => c.Int(nullable: true));
            // Copy existing Student data into new Person table.
            Sql("INSERT INTO dbo.People (LastName, FirstName, HireDate, EnrollmentDate, Discriminator, OldId) SELECT " +
        "LastName, FirstName, null AS HireDate, EnrollmentDate, 'Student' AS Discriminator, StudentID AS OldId FROM dbo.Students");
            // Fix up existing relationships to match new PK's.
            Sql("UPDATE dbo.Enrollments SET StudentId = (SELECT ID FROM dbo.People WHERE OldId = Enrollments.StudentId " +
        "AND Discriminator = 'Student')");
            // Remove temporary key
            DropColumn("dbo.People", "OldId");
            DropTable("dbo.Students");
            // Re-create foreign keys and indexes pointing to new table.
            AddForeignKey("dbo.Enrollments", "StudentID", "dbo.People", "ID", cascadeDelete: true);
            CreateIndex("dbo.Enrollments", "StudentID");

        }

        public override void Down()
        {
            CreateTable(
                "dbo.Students",
                c => new
                    {
                        StudentID = c.Int(nullable: false, identity: true),
                        LastName = c.String(nullable: false, maxLength: 30),
                        FirstName = c.String(nullable: false, maxLength: 30),
                        EnrollmentDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.StudentID);

            CreateTable(
                "dbo.Instructors",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        LastName = c.String(maxLength: 50),
                        FirstName = c.String(maxLength: 50),
                        HireDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);

            DropForeignKey("dbo.Enrollments", "StudentID", "dbo.People");
            DropIndex("dbo.Enrollments", new[] { "StudentID" });
            DropTable("dbo.People");
            CreateIndex("dbo.Enrollments", "StudentID");
            AddForeignKey("dbo.Enrollments", "StudentID", "dbo.Students", "StudentID", cascadeDelete: true);
        }
    }
}
