using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace InscriptionsApi.Models;

public partial class Credential
{
    public int UserId { get; set; }

    public string UserPassword { get; set; } = null!;

    public string CredentialSalt { get; set; } = null!;


    
    public virtual User User { get; set; } = null!;

}
