using System;
using System.Collections.Generic;

namespace InscriptionsApiLocal.Models;

public partial class Inscription
{
    public int IncriptionId { get; set; }

    public int StudentId { get; set; }

    public int SubjectId { get; set; }

    public DateTime IncriptionDate { get; set; }

    public virtual Student Student { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;
}
