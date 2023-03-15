using System;
using System.Collections.Generic;

namespace InscriptionsApiLocal.Models;

public partial class Student
{
    public int StudentId { get; set; }

    public string StudentName { get; set; } = null!;

    public string StudentLn { get; set; } = null!;

    public string TypeDocStudent { get; set; } = null!;

    public string StudentDoc { get; set; } = null!;

    public int StudentStatus { get; set; }

    public string StudentGenre { get; set; } = null!;

    public virtual ICollection<Inscription> Inscriptions { get; } = new List<Inscription>();
}
