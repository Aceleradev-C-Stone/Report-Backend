using System;
using Report.Core.Enums;

namespace Report.Core.Dto.Responses
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public EUserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}