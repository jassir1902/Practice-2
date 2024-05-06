using Microsoft.AspNetCore.Mvc;
using UPB.BusinessLogic;
using UPB.BusinessLogic.Exceptions;
using UPB.BusinessLogic.Managers;
using UPB.BusinessLogic.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Practice2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {

        private readonly IPatientManager _patientManager;

        public PatientsController(IPatientManager patientManager)
        {
            _patientManager = patientManager;
        }


        // GET: api/<PatientsController>
        /*[HttpGet]
        public IEnumerable<Patient> GetPatients()
        {
            return _patientManager.GetAll();
        }*/

        [HttpGet]
        public ActionResult<IEnumerable<Patient>> GetPatients()
        {
            try
            {
                var patients = _patientManager.GetAll();
                return Ok(patients);
            }
            catch (NotFoundException ex)
            {
                return NotFound($"Resource not found: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("{ci}")]
        public ActionResult<Patient> GetPatientsById(string ci)
        {
            try
            {
                Patient patient = _patientManager.Get(ci);
                if (patient == null)
                {
                    return NotFound($"Patient with ID '{ci}' not found");
                }
                return Ok(patient);
            }
            catch (NotFoundException ex)
            {
                return NotFound($"Resource not found: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public ActionResult<Patient> Post([FromBody] Patient patient)
        {
            try
            {
                var bloodGroup = _patientManager.GenerateRandomBloodGroup();
                patient.BloodGroup = bloodGroup;
                _patientManager.Create(patient);
                return patient;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPut("{ci}")]
        public ActionResult<Patient> Put(string ci, [FromBody] Patient patient)
        {
            try
            {
                var existingPatient = _patientManager.Get(ci);
                if (existingPatient == null)
                {
                    return NotFound($"Patient with ID '{ci}' not found");
                }

                existingPatient.Name = patient.Name;
                existingPatient.LastName = patient.LastName;
                _patientManager.Update(existingPatient);
                return Ok(patient);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{ci}")]
        public ActionResult<Patient> Delete(string ci)
        {
            try
            {
                var existingPatient = _patientManager.Get(ci);
                if (existingPatient == null)
                {
                    return NotFound($"Patient with ID '{ci}' not found");
                }

                _patientManager.Delete(ci);
                return Ok(existingPatient);
            }
            catch (NotFoundException ex)
            {
                return NotFound($"Resource not found: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
    

