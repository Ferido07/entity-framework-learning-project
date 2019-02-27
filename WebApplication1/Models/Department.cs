using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace WebApplication1.Models
{
    public class Department
    {
        public int DepartmentID { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        public decimal Budget { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        /* By convention, the Entity Framework enables cascade delete for non-nullable foreign keys and for many-to-many
            relationships. This can result in circular cascade delete rules, which will cause an exception when you try to add a
            migration. For example, if you didn't define the Department.InstructorID  property as nullable, you'd get the
            following exception message: "The referential relationship will result in a cyclical reference that's not allowed." If your
            business rules required InstructorID  property to be non-nullable, you would have to use the following fluent API
            statement to disable cascade delete on the relationship in SchoolContext:
         * 
         * modelBuilder.Entity().HasRequired(d => d.Administrator).WithMany().WillCascadeOnDelete(false);
         */
        [Display(Name="Administrator")]
        public int? InstructorID { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }


        public virtual Instructor Administrator { get; set; }
        public virtual ICollection<Course> Courses { get; set; }

    }
}
