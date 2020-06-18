using System;
using System.Collections.Generic;
using Report.Domain.Enums;

namespace Report.Domain.Models
{
    public class User
    {
        public int Id { get; private set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Hash { get; set; }
        public string Salt { get; set; }
        public EUserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Log> Logs { get; set; }
    }
}