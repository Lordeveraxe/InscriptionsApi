using InscriptionsApiLocal.Models;
using System.Collections.Generic;

namespace InscriptionsApi.Controllers.DTO
{
    public class InscriptionWithNames
    {
      
            public int IncriptionId { get; set; }

            public string StudentName { get; set; }

            public string SubjectName { get; set; }

            public DateTime IncriptionDate { get; set; }
        
    }
}
