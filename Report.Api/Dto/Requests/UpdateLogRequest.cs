using System.ComponentModel.DataAnnotations;
using Report.Domain.Enums;

namespace Report.Api.Dto.Requests
{
    public class UpdateLogRequest
    {
        [Required(ErrorMessage="A descrição é necessária")]
        [StringLength(255, ErrorMessage="A descrição precisa ter no máximo {1} caracteres")]
        public string Description { get; set; }

        [Required(ErrorMessage="O título é necessário")]
        [StringLength(60, ErrorMessage="O título precisa ter no máximo {1} caracteres")]
        public string Title { get; set; }

        [Required(ErrorMessage="O campo details é necessário")]
        public string Details { get; set; }

        [Required(ErrorMessage="A origem é necessária")]
        [StringLength(60, ErrorMessage="A origem precisa ter no máximo {1} caracteres")]
        public string Source { get; set; }

        [Required(ErrorMessage="A quantidade de eventos é necessária")]
        [Range(1, int.MaxValue, ErrorMessage="Deve ter pelo menos um evento no log")]
        public int EventCount { get; set; }

        [Required(ErrorMessage="O nível do log é necessário")]
        public ELogLevel Level { get; set; }

        [Required(ErrorMessage="O canal do log é necessário")]
        public ELogChannel Channel { get; set; }
    }
}