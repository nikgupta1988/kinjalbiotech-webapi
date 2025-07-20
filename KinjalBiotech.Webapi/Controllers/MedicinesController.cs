using KinjalBiotech.Webapi.Data;
using KinjalBiotech.Webapi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KinjalBiotech.Webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicinesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MedicinesController> _logger;
        
        public MedicinesController(ApplicationDbContext context, ILogger<MedicinesController> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        /// <summary>
        /// Get all medicines
        /// </summary>
        /// <returns>List of medicines</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Medicine>>> GetMedicines()
        {
            try
            {
                var medicines = await _context.Medicines
                    .Include(m => m.Department)
                    .ToListAsync();
                
                _logger.LogInformation("Retrieved {Count} medicines", medicines.Count);
                return Ok(medicines);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving medicines");
                return StatusCode(500, "An error occurred while retrieving medicines.");
            }
        }
        
        /// <summary>
        /// Get medicine by ID
        /// </summary>
        /// <param name="id">Medicine ID</param>
        /// <returns>Medicine details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Medicine>> GetMedicine(int id)
        {
            try
            {
                var medicine = await _context.Medicines
                    .Include(m => m.Department)
                    .FirstOrDefaultAsync(m => m.MedicineID == id);
                
                if (medicine == null)
                {
                    _logger.LogWarning("Medicine with ID {Id} not found", id);
                    return NotFound($"Medicine with ID {id} not found.");
                }
                
                _logger.LogInformation("Retrieved medicine with ID {Id}", id);
                return Ok(medicine);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving medicine with ID {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the medicine.");
            }
        }

        /// <summary>
        /// Get medicines by department ID
        /// </summary>
        /// <param name="deptId">Department ID</param>
        /// <returns>List of medicines in the department</returns>
        [HttpGet("department/{deptId}")]
        public async Task<ActionResult<IEnumerable<Medicine>>> GetMedicinesByDepartment(int deptId)
        {
            try
            {
                var medicines = await _context.Medicines
                    .Include(m => m.Department)
                    .Where(m => m.DeptID == deptId)
                    .ToListAsync();
                
                _logger.LogInformation("Retrieved {Count} medicines for department {DeptId}", medicines.Count, deptId);
                return Ok(medicines);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving medicines for department {DeptId}", deptId);
                return StatusCode(500, "An error occurred while retrieving medicines for the department.");
            }
        }

        /// <summary>
        /// Create a new medicine
        /// </summary>
        /// <param name="medicine">Medicine details</param>
        /// <returns>Created medicine</returns>
        [HttpPost]
        public async Task<ActionResult<Medicine>> CreateMedicine([FromBody] Medicine medicine)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Validate that the department exists
                var department = await _context.Departments.FindAsync(medicine.DeptID);
                if (department == null)
                {
                    _logger.LogWarning("Department with ID {DeptId} does not exist", medicine.DeptID);
                    return BadRequest($"Department with ID {medicine.DeptID} does not exist.");
                }
                
                medicine.UpdatedDate = DateTime.UtcNow;
                _context.Medicines.Add(medicine);
                await _context.SaveChangesAsync();
                
                // Load the department for the response
                await _context.Entry(medicine)
                    .Reference(m => m.Department)
                    .LoadAsync();
                
                _logger.LogInformation("Created medicine with ID {Id}", medicine.MedicineID);
                return CreatedAtAction(nameof(GetMedicine), new { id = medicine.MedicineID }, medicine);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating medicine");
                return StatusCode(500, "An error occurred while creating the medicine.");
            }
        }
        
        /// <summary>
        /// Update an existing medicine
        /// </summary>
        /// <param name="id">Medicine ID</param>
        /// <param name="medicine">Updated medicine details</param>
        /// <returns>Updated medicine</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<Medicine>> UpdateMedicine(int id, [FromBody] Medicine medicine)
        {
            if (id != medicine.MedicineID)
            {
                return BadRequest("ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingMedicine = await _context.Medicines.FindAsync(id);
                if (existingMedicine == null)
                {
                    _logger.LogWarning("Medicine with ID {Id} not found for update", id);
                    return NotFound($"Medicine with ID {id} not found.");
                }

                // Validate department existence if updating DeptID
                if (existingMedicine.DeptID != medicine.DeptID)
                {
                    var department = await _context.Departments.FindAsync(medicine.DeptID);
                    if (department == null)
                    {
                        _logger.LogWarning("Department with ID {DeptId} does not exist", medicine.DeptID);
                        return BadRequest($"Department with ID {medicine.DeptID} does not exist.");
                    }
                }

                existingMedicine.DeptID = medicine.DeptID;
                existingMedicine.MedicineName = medicine.MedicineName;
                existingMedicine.MedicineDesc = medicine.MedicineDesc;
                existingMedicine.MedicineQuantity = medicine.MedicineQuantity;
                existingMedicine.ImageUrl = medicine.ImageUrl;
                existingMedicine.UpdatedBy = medicine.UpdatedBy;
                existingMedicine.UpdatedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                
                // Load the department for the response
                await _context.Entry(existingMedicine)
                    .Reference(m => m.Department)
                    .LoadAsync();
                
                _logger.LogInformation("Updated medicine with ID {Id}", id);
                return Ok(existingMedicine);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error updating medicine with ID {Id}", id);
                return StatusCode(500, "A concurrency error occurred during update.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating medicine with ID {Id}", id);
                return StatusCode(500, "An error occurred while updating the medicine.");
            }
        }
        
        /// <summary>
        /// Delete a medicine
        /// </summary>
        /// <param name="id">Medicine ID</param>
        /// <returns>No content</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedicine(int id)
        {
            try
            {
                var medicine = await _context.Medicines.FindAsync(id);
                if (medicine == null)
                {
                    _logger.LogWarning("Medicine with ID {Id} not found for deletion", id);
                    return NotFound($"Medicine with ID {id} not found.");
                }
                
                _context.Medicines.Remove(medicine);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Deleted medicine with ID {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting medicine with ID {Id}", id);
                return StatusCode(500, "An error occurred while deleting the medicine.");
            }
        }
    }
}
