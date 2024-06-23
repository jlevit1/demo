using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace demo.Models
{
    public class Recuiter
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Image { get; set; }
        public User User { get; set; }
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Recuiter name must be 3 to 30 characters")]
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        [Column("Scale", TypeName = "Numeric"), Range(1, 3000, ErrorMessage = "Scale must be from 1 to 3000")]
        public decimal Scale { get; set; }
        public string Description { get; set; }

        public ICollection<Job> Jobs { get; set; }= new List<Job>();
    }
}
