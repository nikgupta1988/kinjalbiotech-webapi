using System.ComponentModel.DataAnnotations;

namespace KinjalBiotech.Webapi.Models
{
    public class Department
    {
        [Key]
        public int DeptID { get; set; }

        [Required]
        [StringLength(100)]
        public string DeptName { get; set; } = string.Empty;

        [StringLength(50)]
        public string? UpdatedBy { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}
