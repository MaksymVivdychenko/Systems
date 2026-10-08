using System;
using System.Collections.Generic;

namespace lab_3.Task2
{
    public class Task2Runner
    {
        public static void Run()
        {
            Console.WriteLine("\n================================================================================");
            Console.WriteLine("                       ЗАВДАННЯ 2: СМО БАНКУ (АВТОМОБІЛІ)                       ");
            Console.WriteLine("================================================================================");

            // 1. Ініціалізація
            Create creator = new Create(0.5);
            creator.SetName("CAR_GENERATOR");
            creator.SetDistribution("exp");
            // Час першої появи = 0.1
            creator.SetTnext(0.1);

            BankLane lane1 = new BankLane(1.0, channels: 1);
            lane1.SetName("LANE_1");
            lane1.SetDistribution("norm");
            lane1.SetDelayDev(0.3);
            lane1.SetMaxqueue(3); 

            BankLane lane2 = new BankLane(1.0, channels: 1);
            lane2.SetName("LANE_2");
            lane2.SetDistribution("norm");
            lane2.SetDelayDev(0.3);
            lane2.SetMaxqueue(3);

            // Initial state
            lane1.SetInitialBankState(initialDelayMean: 1.0, initialDelayDev: 0.3, initialQueueSize: 2);
            lane2.SetInitialBankState(initialDelayMean: 1.0, initialDelayDev: 0.3, initialQueueSize: 2);
            
            // Adjacent lanes
            lane1.AdjacentLane = lane2;
            lane2.AdjacentLane = lane1;

            Dispose dispose = new Dispose();
            dispose.SetName("DISPOSE");

            // Routing
            creator.AddNextElement(lane1, priority: 1);
            creator.AddNextElement(lane2, priority: 2);
            creator.SetRouteMode(RouteMode.ByQueuePriority);

            lane1.SetNextElement(dispose);
            lane2.SetNextElement(dispose);

            List<Element> elements = new List<Element> { creator, lane1, lane2, dispose };
            Model model = new Model(elements);
            model.Simulate(time: 1000.0, printEvents: false);

            Console.WriteLine($"\nBank Simulation Results:");
            Console.WriteLine($"Lane 1: Served = {lane1.GetQuantity()}, Failures = {lane1.GetFailure()}, Lane Switches = {lane1.LaneSwitchesCount}");
            Console.WriteLine($"Lane 2: Served = {lane2.GetQuantity()}, Failures = {lane2.GetFailure()}, Lane Switches = {lane2.LaneSwitchesCount}");
            Console.WriteLine($"Total failures: {lane1.GetFailure() + lane2.GetFailure()}");
            Console.WriteLine($"Average time in bank: {dispose.GetAverageTimeInSystem():F3}");
            Console.WriteLine($"Average departure interval: {dispose.GetAverageDepartureInterval():F3}");
            Console.WriteLine($"Average clients in bank: {model.totalClientsInSystemMean / 1000.0:F3}");
            Console.WriteLine("================================================================================");
        }
    }
}
