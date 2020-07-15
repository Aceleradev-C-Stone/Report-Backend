using Report.Core.Enums;

namespace Report.Core.Dto.Requests
{
    public class CreateLogRequest
    {
        public string Description { get; set; }
        public string Title { get; set; }
        public string Details { get; set; }
        public string Source { get; set; }
        public int EventCount { get; set; }
        public ELogLevel Level { get; set; }
        public ELogChannel Channel { get; set; }
        public int? UserId { get; set; }
    }
}