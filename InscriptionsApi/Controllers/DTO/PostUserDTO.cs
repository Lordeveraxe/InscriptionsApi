namespace InscriptionsApi.Controllers.DTO
{
    public class PostUserDTO
    {
        public string UserName { get; set; } = null!;

        public string UserEmail { get; set; } = null!;

        public int UserState { get; set; }

        public string UserPassword { get; set; } = null!;
    }
}
