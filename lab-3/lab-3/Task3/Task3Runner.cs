using System;
using System.Collections.Generic;

namespace lab_3.Task3
{
    public class Task3Runner
    {
        public static void Run()
        {
            Console.WriteLine("\n================================================================================");
            Console.WriteLine("                       ЗАВДАННЯ 3: СМО ЛІКАРНІ                                  ");
            Console.WriteLine("================================================================================");

            PatientCreator creator = new PatientCreator(15.0);
            creator.SetName("PATIENT_GENERATOR");
            creator.SetDistribution("exp");

            ReceptionProcess reception = new ReceptionProcess(2);
            reception.SetName("RECEPTION");
            reception.SetMaxqueue(int.MaxValue);

            Process escort = new Process(delay: 3.0, channels: 3); // Unif 3..8. Min=3, Max=8
            escort.SetName("ESCORT");
            escort.SetDistribution("unif");
            escort.SetDelayDev(8.0);

            Process walkToLab = new Process(delay: 2.0, channels: 1000); 
            walkToLab.SetName("WALK_TO_LAB");
            walkToLab.SetDistribution("unif");
            walkToLab.SetDelayDev(5.0);

            LabRegistryProcess labRegistry = new LabRegistryProcess(4.5, channels: 1);
            labRegistry.SetName("LAB_REGISTRY");
            labRegistry.SetDistribution("erlang");
            labRegistry.SetDelayDev(3);

            LabAnalysisProcess labAnalysis = new LabAnalysisProcess(4.0, channels: 2);
            labAnalysis.SetName("LAB_ANALYSIS");
            labAnalysis.SetDistribution("erlang");
            labAnalysis.SetDelayDev(2);

            Process walkToReception = new Process(delay: 2.0, channels: 1000);
            walkToReception.SetName("WALK_TO_RECEPTION");
            walkToReception.SetDistribution("unif");
            walkToReception.SetDelayDev(5.0);

            Dispose disposeWard = new Dispose();
            disposeWard.SetName("DISPOSE_WARD");

            Dispose disposeExit = new Dispose();
            disposeExit.SetName("DISPOSE_EXIT");

            // Routing
            creator.SetNextElement(reception);
            
            reception.AddNextElement(escort);      // Route 0 (Type 1)
            reception.AddNextElement(walkToLab);   // Route 1 (Type 2, 3)

            escort.SetNextElement(disposeWard);

            walkToLab.SetNextElement(labRegistry);
            labRegistry.SetNextElement(labAnalysis);

            labAnalysis.AddNextElement(walkToReception); // Route 0 (Type 2 -> 1)
            labAnalysis.AddNextElement(disposeExit);     // Route 1 (Type 3)

            walkToReception.SetNextElement(reception);

            List<Element> elements = new List<Element> 
            { 
                creator, reception, escort, walkToLab, labRegistry, 
                labAnalysis, walkToReception, disposeWard, disposeExit 
            };
            
            Model model = new Model(elements);
            model.Simulate(time: 10000.0, printEvents: false); // run for 10000 mins for better statistics

            Console.WriteLine($"\nHospital Simulation Results:");
            Console.WriteLine($"Total patients generated: {creator.GetQuantity()}");
            Console.WriteLine($"Admitted to hospital (Type 1 & 2): {disposeWard.GetQuantity()}");
            Console.WriteLine($"Left hospital after check (Type 3): {disposeExit.GetQuantity()}");
            
            double totalProcessed = disposeWard.GetQuantity() + disposeExit.GetQuantity();
            double totalTime = (disposeWard.GetAverageTimeInSystem() * disposeWard.GetQuantity() + disposeExit.GetAverageTimeInSystem() * disposeExit.GetQuantity()) / totalProcessed;
            
            Console.WriteLine($"\nAverage time in system: {totalTime:F2} min");
            Console.WriteLine($"Average interval of arrivals to lab: {labRegistry.GetAverageArrivalInterval():F2} min");
            Console.WriteLine($"Average clients in reception queue: {reception.GetMeanQueue() / 10000.0:F2}");
            Console.WriteLine($"Average clients in lab registry queue: {labRegistry.GetMeanQueue() / 10000.0:F5}");
            Console.WriteLine($"Average clients in lab analysis queue: {labAnalysis.GetMeanQueue() / 10000.0:F5}");
            Console.WriteLine($"Average escort queue (wait for escort in reception): {escort.GetMeanQueue() / 10000.0:F5}");
            Console.WriteLine("================================================================================");
        }
    }
}
