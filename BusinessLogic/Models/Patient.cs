﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPB.BusinessLogic.Models
{
    public class Patient
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string CI { get; set; }
        public string BloodGroup { get; set; }
        public string PatientCode { get; set; }
    }
    public class PatientData
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string CI { get; set; }
    }
}
