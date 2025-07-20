using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KinjalBiotech.Webapi.Models
{
    public class Medicine
    {
        [Key]
        public int MedicineID { get; set; }

        [ForeignKey("Department")]
        public int DeptID { get; set; }
        
        [Required]
        [StringLength(200)]
        public string MedicineName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? MedicineDesc { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a positive number")]
        public int MedicineQuantity { get; set; }
        
        [StringLength(500)]
        public string? ImageUrl { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [StringLength(50)]
        public string? UpdatedBy { get; set; }

        // Navigation property
        public virtual Department? Department { get; set; }
    }
}
