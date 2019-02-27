using System.Data.Entity;
using WebApplication1.Models;
namespace WebApplication1.DAL
{
    public class SchoolContext : DbContext
    {
        //The database creation by this class would work the same if 2 of the 3
        //DbSet properties where not included because either 1 references 1 of the 
        //other 2 and the referenced again refences the next class and so DbContext
        //(SchoolContext adds them all because of thier dependencies.

        public SchoolContext()
            : base("SchoolContext")
        {
           /* To diasable lazy loading for all navigation properties  
            * For all navigation properties, set LazyLoadingEnabled  to false , put the following code in the constructor of
                your context class:
                this.Configuration.LazyLoadingEnabled = false;
            */
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<OfficeAssignment> OfficeAssignments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //this code removes the convention of adding 's' ie. pluralizing when
            //creating the database tables whic is the default
            //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //fluent api to create a many-to-many relationship table  named CourseInstructor
            //between course and instructor containting only their 
            //primary keys as foreign key to the corresponding tables 
            /* Code First can configure the many-to-many relationship for you
             * without this code, but if you don't call it, you will get default names such as InstructorInstructorID  for the 
             * InstructorID  column.
             */
            modelBuilder.Entity<Course>()
            .HasMany(c => c.Instructors).WithMany(i => i.Courses)
            .Map(t => t.MapLeftKey("CourseID")
                .MapRightKey("InstructorID")
                .ToTable("CourseInstructor"));

            /* The following code provides an example of how you could have used fluent API instead of attributes(Key and ForeignKey to specify the
             * relationship between the Instructor  and OfficeAssignment  entities:
             *
             *      modelBuilder.Entity<Instructor>()
             *      .HasOptional(p => p.OfficeAssignment).WithRequired(p => p.Instructor);*/


            modelBuilder.Entity<Department>().MapToStoredProcedures();
        }

    }
}