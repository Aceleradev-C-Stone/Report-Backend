using System.ComponentModel.DataAnnotations;

namespace Report.Api.Dto.Requests
{
    public class CreateUserRequest
    {
        [Required(ErrorMessage="O nome é necessário")]
        [StringLength(60, ErrorMessage="O nome precisa ter no máximo {1} caracteres")]
        public string Name { get; set; }

        [Required(ErrorMessage="O email é necessário")]
        [EmailAddress(ErrorMessage="O email está em um formato incorreto")]
        [StringLength(60, ErrorMessage="O email precisa ter no máximo {1} caracteres")]
        public string Email { get; set; }

        [Required(ErrorMessage="A senha é necessária")]
        [StringLength(30, ErrorMessage="A senha precisa ter no máximo {1} caracteres")]
        public string Password { get; set; }
    }
}