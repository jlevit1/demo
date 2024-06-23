using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace demo.Models
{
    public class Job
    {
        public int Id { get; set; }
        [StringLength(255, MinimumLength = 3, ErrorMessage = "Product name must be 3 to 30 characters")]
        public string Name { get; set; }
        [StringLength(255, MinimumLength = 3, ErrorMessage = "Product name must be 3 to 30 characters")]
        public string Skills { get; set; }
        [StringLength(255, MinimumLength = 3, ErrorMessage = "Product name must be 3 to 30 characters")]
        public string Requiment { get; set; }
        public DateTime DueDate { get; set; }
        [Column("Salary", TypeName = "Numeric"), Range(1, 100000, ErrorMessage = "Salary must be from 1$ to 100000$")]
        public decimal Salary { get; set; }
        [StringLength(10000, MinimumLength = 3, ErrorMessage = "Product name must be 3 to 30 characters")]
        public string Description { get; set; }
        public int RecuiterId { get; set; }
        public Recuiter Recuiter { get; set; }
        public ICollection<JobCandidate> JobCandidates { get; set; }= new List<JobCandidate>();
    }
}
