using HealthTrack.Core.Models.Enums;
using HealthTrack.Core.Utils;
using System.ComponentModel.DataAnnotations;

namespace HealthTrack.Core.Models.Entities
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Cpf { get; set; }

        [StringLength(20)]
        public string? Rg { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public Gender Gender { get; set; } = Gender.NotDefined;

        [StringLength(15)]
        public string? Phone { get; set; }

        [StringLength(256)]
        public string? Email { get; set; }

        public BloodType BloodType { get; set; } = BloodType.NotDefined;

        [StringLength(500)]
        public string? MedicalHistory { get; set; }

        public DateTime CreatedAt { get; set; } = DateTimeHelper.Now;
        public DateTime UpdatedAt { get; set; } = DateTimeHelper.Now;

        // Domain methods
        public void SetForCreation()
        {
            var now = DateTimeHelper.Now;
            CreatedAt = now;
            UpdatedAt = now;
        }

        public void SetForUpdate()
        {
            UpdatedAt = DateTimeHelper.Now;
        }

        public string GetFullName()
        {
            return $"{FirstName} {LastName}";
        }

        public void Update(HealthTrack.Areas.Patient.ViewModels.EditPatientViewModel model)
        {
            FirstName = model.FirstName;
            LastName = model.LastName;
            Cpf = model.Cpf;
            Rg = model.Rg;
            DateOfBirth = model.DateOfBirth;
            Gender = model.Gender;
            Phone = model.Phone;
            Email = model.Email;
            BloodType = model.BloodType;
            MedicalHistory = model.MedicalHistory;
        }

        public string? UserId { get; set; }
        public virtual User? User { get; set; }

        public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();
    }
}