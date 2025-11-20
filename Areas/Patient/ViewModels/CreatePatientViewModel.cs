using HealthTrack.Core.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace HealthTrack.Areas.Patient.ViewModels
{
    public class CreatePatientViewModel
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sobrenome é obrigatório")]
        [StringLength(100, ErrorMessage = "Sobrenome deve ter no máximo 100 caracteres")]
        public string LastName { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "CPF deve ter no máximo 20 caracteres")]
        public string? Cpf { get; set; }

        [StringLength(20, ErrorMessage = "RG deve ter no máximo 20 caracteres")]
        public string? Rg { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public Gender Gender { get; set; } = Gender.NotDefined;

        [StringLength(15, ErrorMessage = "Telefone deve ter no máximo 15 caracteres")]
        public string? Phone { get; set; }

        [StringLength(256, ErrorMessage = "Email deve ter no máximo 256 caracteres")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string? Email { get; set; }

        public BloodType BloodType { get; set; } = BloodType.NotDefined;

        [StringLength(500, ErrorMessage = "Histórico médico deve ter no máximo 500 caracteres")]
        public string? MedicalHistory { get; set; }
    }
}