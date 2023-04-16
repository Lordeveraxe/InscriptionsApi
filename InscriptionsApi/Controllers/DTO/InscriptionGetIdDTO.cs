 namespace InscriptionsApi.Controllers.DTO
{
    public class InscriptionGetIdDTO
    {
        public int IncriptionId { get; set; }
        public int StudentId { get; set; }
        public String SubjectName { get; set; } = null!;
    }
}
