namespace InscriptionsApi.Controllers.DTO
{
    public class PutUserDataDTO

    {
        public string UserName { get; set; } = null!;

        public string UserEmail { get; set; } = null!;

        public int UserState { get; set; }

        public string PreviousPassword { get; set; } = null!;

   
    }
}
