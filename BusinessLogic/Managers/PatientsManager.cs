using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using UPB.BusinessLogic.Exceptions;
using UPB.BusinessLogic.Models;

namespace UPB.BusinessLogic.Managers
{
    public interface IPatientManager
    {
        void Create(Patient patient);
        void Update(Patient patient);
        void Delete(string ci);
        Patient Get(string ci);
        IEnumerable<Patient> GetAll();
        string GenerateRandomBloodGroup();
    }

    public class PatientManager : IPatientManager
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly IConfiguration _configuration;

        public PatientManager(IConfiguration configuration, IFileStorageService fileStorageService)
        {
            _configuration = configuration;
            _fileStorageService = fileStorageService;
        }

        public void Create(Patient patient)
        {
            try
            {
                var patients = _fileStorageService.LoadPatients();
                patients.Add(patient);
                _fileStorageService.SavePatients(patients);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while creating the patient: {ErrorMessage}", ex.Message);
                throw new NotFoundException($"Error creating patient.");
            }
        }

        public void Update(Patient patient)
        {
            try
            {
                var patients = _fileStorageService.LoadPatients();
                var existingPatient = patients.Find(p => p.CI == patient.CI);
                if (existingPatient != null)
                {
                    existingPatient.Name = patient.Name;
                    existingPatient.LastName = patient.LastName;
                    _fileStorageService.SavePatients(patients);
                }
                else
                {
                    throw new NotFoundException($"Patient with CI {patient.CI} not found.");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while updating the patient: {ErrorMessage}", ex.Message);
                throw new NotFoundException("Error updating patient.");
            }
        }

        public void Delete(string ci)
        {
            try
            {
                var patients = _fileStorageService.LoadPatients();
                var existingPatient = patients.Find(p => p.CI == ci);
                if (existingPatient != null)
                {
                    patients.Remove(existingPatient);
                    _fileStorageService.SavePatients(patients);
                }
                else
                {
                    throw new NotFoundException($"Patient with CI {ci} not found.");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while deleting the patient: {ErrorMessage}", ex.Message);
                throw new NotFoundException("Error deleting patient.");
            }
        }

        public Patient Get(string ci)
        {
            try
            {
                var patients = _fileStorageService.LoadPatients();
                Patient patient = patients.Find(p => p.CI == ci);
                if (patient == null)
                {
                    throw new NotFoundException($"Patient with CI {ci} not found.");
                }
                return patient;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while retrieving the patient: {ErrorMessage}", ex.Message);
                throw new NotFoundException("Error retrieving patient.");
            }
        }

        public IEnumerable<Patient> GetAll()
        {
            try
            {
                return _fileStorageService.LoadPatients();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while retrieving all patients: {ErrorMessage}", ex.Message);
                throw new NotFoundException("Error retrieving all patients.");
            }
        }

        public string GenerateRandomBloodGroup()
        {
            string[] bloodGroups = { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };
            Random random = new Random();
            int index = random.Next(bloodGroups.Length);
            return bloodGroups[index];
        }
    }
}
