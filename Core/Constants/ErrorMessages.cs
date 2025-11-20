namespace HealthTrack.Core.Constants
{
    public static class ErrorMessages
    {
        public const string PatientNotFound = "Paciente não encontrado";
        public const string ExamNotFound = "Exame não encontrado ou não pertence ao paciente";
        public const string ExamNameRequired = "Nome do exame é obrigatório";
        public const string ParameterNameRequired = "Nome do parâmetro é obrigatório";
        public const string UserIdRequired = "UserId não pode ser nulo ou vazio";
        public const string InvalidId = "Id deve ser maior que zero";
    }
}