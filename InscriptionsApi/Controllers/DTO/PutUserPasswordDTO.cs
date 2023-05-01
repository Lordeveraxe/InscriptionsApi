namespace InscriptionsApi.Controllers.DTO
{
    public class PutUserPasswordDTO
    {
 
        public string PreviousPassword { get; set; } = null!;

        public string NewPassword { get; set; } = null!;
    }
}
