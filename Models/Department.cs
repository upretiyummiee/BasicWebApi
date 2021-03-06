using BasicWebApi.Data;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BasicWebApi.Models
{
    [Authorize]
    public class Department
    {

        [Key]
        public int Id { get; set; }

        [MaxLength(20)]
        public string Depart { get; set; }

        public virtual ICollection<Employee> Employees { get; set; } = new HashSet<Employee>();

    }
}
