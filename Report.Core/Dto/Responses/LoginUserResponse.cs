using Report.Core.Dto.Responses;

namespace Report.Core.Dto.Responses
{
    public class LoginUserResponse
    {
        public UserResponse User { get; set; }
        public string Token { get; set; }
        public int ExpiresIn { get; set; }
    }
}