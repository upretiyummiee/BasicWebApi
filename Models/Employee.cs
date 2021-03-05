using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BasicWebApi.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [StringLength(25)]
        public String Name { get; set; }

        [MaxLength(30)]
        public String Address { get; set; }


        [ForeignKey("Department")]
        public int DepartmentId { get; set; }

        public virtual Department Department { get; set; }
    }
}
