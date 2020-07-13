using System;
using System.Collections.Generic;
using Report.Core.Enums;

namespace Report.Core.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Hash { get; set; }
        public string Salt { get; set; }
        public EUserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Log> Logs { get; set; }
    }
}