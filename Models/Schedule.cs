using System.ComponentModel.DataAnnotations;

namespace Fresh_University_Enrollment.Models;

public class Schedule
{
    public int SchdId { get; set; }
    
    [Required]
    public string CrsCode { get; set; }
    public string Room { get; set; }
    
    public string Description { get; set; }
}