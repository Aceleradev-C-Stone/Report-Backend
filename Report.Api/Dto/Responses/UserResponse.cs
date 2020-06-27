using System;
using Report.Domain.Enums;

namespace Report.Api.Dto.Responses
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