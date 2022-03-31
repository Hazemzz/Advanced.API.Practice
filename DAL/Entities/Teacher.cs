using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Advanced.API.Practice.Entities
{
    public class Teacher
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        public ICollection<Course> Courses { get; set; }
            = new List<Course>();
    }
}
