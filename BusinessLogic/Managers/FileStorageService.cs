using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UPB.BusinessLogic.Models;

namespace UPB.BusinessLogic.Managers
{
    public interface IFileStorageService
    {
        void SavePatients(IEnumerable<Patient> patients);
        List<Patient> LoadPatients();
    }

    public class FileStorageService : IFileStorageService
    {
        private readonly string _filePath;

        public FileStorageService(string filePath)
        {
            _filePath = filePath;

            // Crear la carpeta si no existe
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Crear el archivo si no existe
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "[]"); // Inicializar el archivo con un array JSON vacío
            }
        }

        public void SavePatients(IEnumerable<Patient> patients)
        {
            try
            {
                string json = JsonConvert.SerializeObject(patients);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                // Manejar la excepción aquí, ya sea registrándola o lanzándola nuevamente
                Console.WriteLine($"Error al guardar pacientes: {ex.Message}");
                throw; // Lanza la excepción para que sea manejada en un nivel superior si es necesario
            }
        }

        public List<Patient> LoadPatients()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    string json = File.ReadAllText(_filePath);
                    return JsonConvert.DeserializeObject<List<Patient>>(json);
                }
                else
                {
                    return new List<Patient>();
                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción aquí, ya sea registrándola o lanzándola nuevamente
                Console.WriteLine($"Error al cargar pacientes: {ex.Message}");
                throw; // Lanza la excepción para que sea manejada en un nivel superior si es necesario
            }
        }
    }
}
