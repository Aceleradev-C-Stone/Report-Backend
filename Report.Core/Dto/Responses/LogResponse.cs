using System;
using Report.Core.Enums;

namespace Report.Core.Dto.Responses
{
    public class LogResponse
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public string Details { get; set; }
        public string Source { get; set; }
        public int EventCount { get; set; }
        public ELogLevel Level { get; set; }
        public ELogChannel Channel { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Archived { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
    }
}