using System;
using System.Collections.Generic;

namespace InscriptionsApi.Models;

public partial class User
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string UserEmail { get; set; } = null!;

    public int UserState { get; set; }

    public virtual Credential? Credential { get; set; }
}
