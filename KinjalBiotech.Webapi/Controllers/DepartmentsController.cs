using KinjalBiotech.Webapi.Data;
using KinjalBiotech.Webapi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KinjalBiotech.Webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DepartmentsController> _logger;
        
        public DepartmentsController(ApplicationDbContext context, ILogger<DepartmentsController> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        /// <summary>
        /// Get all departments
        /// </summary>
        /// <returns>List of departments</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartments()
        {
            try
            {
                var departments = await _context.Departments.ToListAsync();
                _logger.LogInformation("Retrieved {Count} departments", departments.Count);
                return Ok(departments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving departments");
                return StatusCode(500, "An error occurred while retrieving departments.");
            }
        }
        
        /// <summary>
        /// Get department by ID
        /// </summary>
        /// <param name="id">Department ID</param>
        /// <returns>Department details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Department>> GetDepartment(int id)
        {
            try
            {
                var department = await _context.Departments.FindAsync(id);
                if (department == null)
                {
                    _logger.LogWarning("Department with ID {Id} not found", id);
                    return NotFound($"Department with ID {id} not found.");
                }
                
                _logger.LogInformation("Retrieved department with ID {Id}", id);
                return Ok(department);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving department with ID {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the department.");
            }
        }

        /// <summary>
        /// Create a new department
        /// </summary>
        /// <param name="department">Department details</param>
        /// <returns>Created department</returns>
        [HttpPost]
        public async Task<ActionResult<Department>> CreateDepartment([FromBody] Department department)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            try
            {
                department.UpdateDate = DateTime.UtcNow;
                _context.Departments.Add(department);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Created department with ID {Id}", department.DeptID);
                return CreatedAtAction(nameof(GetDepartment), new { id = department.DeptID }, department);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating department");
                return StatusCode(500, "An error occurred while creating the department.");
            }
        }
        
        /// <summary>
        /// Update an existing department
        /// </summary>
        /// <param name="id">Department ID</param>
        /// <param name="department">Updated department details</param>
        /// <returns>Updated department</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<Department>> UpdateDepartment(int id, [FromBody] Department department)
        {
            if (id != department.DeptID)
            {
                return BadRequest("ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            try
            {
                var existingDepartment = await _context.Departments.FindAsync(id);
                if (existingDepartment == null)
                {
                    _logger.LogWarning("Department with ID {Id} not found for update", id);
                    return NotFound($"Department with ID {id} not found.");
                }

                existingDepartment.DeptName = department.DeptName;
                existingDepartment.UpdatedBy = department.UpdatedBy;
                existingDepartment.UpdateDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Updated department with ID {Id}", id);
                return Ok(existingDepartment);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error updating department with ID {Id}", id);
                return StatusCode(500, "A concurrency error occurred during update.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating department with ID {Id}", id);
                return StatusCode(500, "An error occurred while updating the department.");
            }
        }
        
        /// <summary>
        /// Delete a department
        /// </summary>
        /// <param name="id">Department ID</param>
        /// <returns>No content</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            try
            {
                var department = await _context.Departments.FindAsync(id);
                if (department == null)
                {
                    _logger.LogWarning("Department with ID {Id} not found for deletion", id);
                    return NotFound($"Department with ID {id} not found.");
                }
                
                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Deleted department with ID {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting department with ID {Id}", id);
                return StatusCode(500, "An error occurred while deleting the department.");
            }
        }
    }
}
