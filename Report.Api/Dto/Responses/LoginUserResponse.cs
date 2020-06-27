namespace Report.Api.Dto.Responses
{
    public class LoginUserResponse
    {
        public UserResponse User { get; set; }
        public string Token { get; set; }
        public int ExpiresIn { get; set; }
    }
}