using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using QAPatient.Services;

// TODO: Replace the following version attributes by creating AssemblyInfo.cs. You can do this in the properties of the Visual Studio project.
[assembly: AssemblyVersion("1.0.0.1")]
[assembly: AssemblyFileVersion("1.0.0.1")]
[assembly: AssemblyInformationalVersion("1.0")]

// TODO: Uncomment the following line if the script requires write access.
[assembly: ESAPIScript(IsWriteable = true)]

namespace QAPatient
{
    class Program
    {
        private static string _mrn;

        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                using (Application app = Application.CreateApplication())
                {
                    Execute(app);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
            }
        }
        static void Execute(Application app)
        {
            // TODO: Add your code here.
            CreateNewPatient();
            if (!String.IsNullOrEmpty(_mrn))
            {
                CopyPatientImage(app);
                app.SaveModifications();
            }
        }

        private static void CopyPatientImage(Application app)
        {
            Patient patient = app.OpenPatientById(_mrn);
            
            if (patient != null)
            {
                patient.BeginModifications();
                // Copy the plan from PH_QA_001 to the new patient.
                Console.WriteLine("Copying image from PH_QA_001");
                StructureSet structureSet = patient.CopyImageFromOtherPatient("PH_QA_001", "PHANTOM", "PHANTOM");
                if (structureSet != null)
                {
                    Console.WriteLine("Image copied successfully.");
                }
                else
                {
                    Console.WriteLine("Failed to copy image.");
                }
            }
        }

        private static void CreateNewPatient()
        {
            //set up ARIA Service
            AriaService.SetInitial();
            //create a new patient. 
            Console.WriteLine("Please enter patient Last Name:");
            string lastName = Console.ReadLine();
            Console.WriteLine("Please enter patient First Name:");
            string firstName = Console.ReadLine();
            Console.WriteLine("Please enter patient MRN:");
            string mrn = Console.ReadLine();
            string output = AriaService.CreatePatient(lastName, firstName, mrn, "5b537037-6891-405d-a9a2-333af5367208");
            if (output.Contains("Success"))
            {
                _mrn = mrn;
            }
            Console.WriteLine(output);

        }
    }
}
