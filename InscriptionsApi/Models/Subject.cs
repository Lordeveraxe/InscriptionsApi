using System;
using System.Collections.Generic;

namespace InscriptionsApiLocal.Models;

public partial class Subject
{
    public int SubjectId { get; set; }

    public string SubjectName { get; set; } = null!;

    public int SubjectCapacity { get; set; }

    public int SubjectStatus { get; set; }

    public virtual ICollection<Inscription> Inscriptions { get; } = new List<Inscription>();
}
