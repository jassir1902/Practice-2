using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using UPB.BusinessLogic;
using Newtonsoft.Json;
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
        private readonly HttpClient _httpClient;

        public PatientsController(IPatientManager patientManager, HttpClient httpClient)
        {
            _patientManager = patientManager;
            _httpClient = httpClient;
        }


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

        /*[HttpPost]
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
        }*/

        [HttpPost]
        public async Task<ActionResult<Patient>> Post([FromBody] Patient patient)
        {
            try
            {
                // Enviamos los datos del paciente al generador de códigos de pacientes en Practice 3
                var patientData = new PatientData
                {
                    Name = patient.Name,
                    LastName = patient.LastName,
                    CI = patient.CI
                };

                var jsonPatientData = JsonConvert.SerializeObject(patientData);              
                var content = new StringContent(jsonPatientData, System.Text.Encoding.UTF8, "application/json");

                //var response = await _httpClient.PostAsync("http://localhost:5165/api/PatientCode/GeneratePatientCode", content);
                var response = await _httpClient.PostAsync("http://localhost:5165/api/PatientCode", content);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "Failed to generate patient code.");
                }

                var patientCode = await response.Content.ReadAsStringAsync();

                // Almacenamos el código de paciente generado junto con los demás datos del paciente
                patient.PatientCode = patientCode;
                
                // Creamos al paciente
                var bloodGroup = _patientManager.GenerateRandomBloodGroup();
                patient.BloodGroup = bloodGroup;
                _patientManager.Create(patient);

                return Ok(patient);
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
                //existingPatient.PatientCode = patient.PatientCode;
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
    

