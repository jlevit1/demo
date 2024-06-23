using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace demo.Models
{
    public class Candidate
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Skills { get; set; }
        public string CV { get; set; }
        public ICollection<JobCandidate> JobCandidates { get; set; } = new List<JobCandidate>();
        
    }
}
