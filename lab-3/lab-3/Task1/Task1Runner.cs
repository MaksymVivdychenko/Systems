using System;
using System.Collections.Generic;

namespace lab_3.Task1
{
    public class Task1Runner
    {
        public static void Run()
        {
            Console.WriteLine("================================================================================");
            Console.WriteLine("                       ЗАВДАННЯ 1: УНІВЕРСАЛЬНИЙ АЛГОРИТМ                       ");
            Console.WriteLine("================================================================================");

            Test_UnifiedRouting_QueuePriority();
            Test_UnifiedRouting_StrictPriority();
            Test_UnifiedRouting_Probability();
        }

        public static void Test_UnifiedRouting_QueuePriority()
        {
            Console.WriteLine("\n--------------------------------------------------------------------------------");
            Console.WriteLine(" ТЕСТ 1: Уніфікована маршрутизація з Create за пріоритетом черг (ByQueuePriority)");
            Console.WriteLine(" Умова: вибір коротшої черги; при рівній довжині — вибір Смуги 1 (Priority = 1)");
            Console.WriteLine("--------------------------------------------------------------------------------");

            Create creator = new Create(1.0);
            creator.SetName("CREATOR");
            creator.SetDistribution("exp");

            Process lane1 = new Process(delay: 2.0, channels: 1);
            lane1.SetName("LANE_1 (Priority 1)");
            lane1.SetDistribution("exp");
            lane1.SetMaxqueue(5);

            Process lane2 = new Process(delay: 2.0, channels: 1);
            lane2.SetName("LANE_2 (Priority 2)");
            lane2.SetDistribution("exp");
            lane2.SetMaxqueue(5);

            Dispose dispose = new Dispose();
            dispose.SetName("DISPOSE");

            creator.AddNextElement(lane1, priority: 1);
            creator.AddNextElement(lane2, priority: 2);
            creator.SetRouteMode(RouteMode.ByQueuePriority);

            lane1.SetNextElement(dispose);
            lane2.SetNextElement(dispose);

            List<Element> elements = new List<Element> { creator, lane1, lane2, dispose };
            Model model = new Model(elements);
            model.Simulate(time: 1000.0, printEvents: false);
        }

        public static void Test_UnifiedRouting_StrictPriority()
        {
            Console.WriteLine("\n--------------------------------------------------------------------------------");
            Console.WriteLine(" ТЕСТ 2: Маршрутизація за суворим пріоритетом (ByPriority - Primary / Backup)");
            Console.WriteLine(" Умова: Primary (Prio 1, maxqueue=2). Overflow -> Backup (Prio 2, maxqueue=10)");
            Console.WriteLine("--------------------------------------------------------------------------------");

            Create creator = new Create(0.8);
            creator.SetName("CREATOR");
            creator.SetDistribution("exp");

            Process primary = new Process(delay: 1.0, channels: 1);
            primary.SetName("PRIMARY_SERVER (Priority 1)");
            primary.SetDistribution("exp");
            primary.SetMaxqueue(2);

            Process backup = new Process(delay: 1.0, channels: 1);
            backup.SetName("BACKUP_SERVER (Priority 2)");
            backup.SetDistribution("exp");
            backup.SetMaxqueue(10);

            Dispose dispose = new Dispose();
            dispose.SetName("DISPOSE");

            creator.AddNextElement(primary, priority: 1);
            creator.AddNextElement(backup, priority: 2);
            creator.SetRouteMode(RouteMode.ByPriority);

            primary.SetNextElement(dispose);
            backup.SetNextElement(dispose);

            List<Element> elements = new List<Element> { creator, primary, backup, dispose };
            Model model = new Model(elements);
            model.Simulate(time: 1000.0, printEvents: false);
        }

        public static void Test_UnifiedRouting_Probability()
        {
            Console.WriteLine("\n--------------------------------------------------------------------------------");
            Console.WriteLine(" ТЕСТ 3: Уніфікована маршрутизація за ймовірністю (ByProbability: 70% / 30%)");
            Console.WriteLine("--------------------------------------------------------------------------------");

            Create creator = new Create(1.0);
            creator.SetName("CREATOR");
            creator.SetDistribution("exp");

            Process branchA = new Process(delay: 1.2, channels: 2);
            branchA.SetName("BRANCH_A (70%)");
            branchA.SetDistribution("exp");
            branchA.SetMaxqueue(5);

            Process branchB = new Process(delay: 1.2, channels: 2);
            branchB.SetName("BRANCH_B (30%)");
            branchB.SetDistribution("exp");
            branchB.SetMaxqueue(5);

            Dispose dispose = new Dispose();
            dispose.SetName("DISPOSE");

            creator.AddNextElement(branchA, probability: 0.70);
            creator.AddNextElement(branchB, probability: 0.30);
            creator.SetRouteMode(RouteMode.ByProbability);

            branchA.SetNextElement(dispose);
            branchB.SetNextElement(dispose);

            List<Element> elements = new List<Element> { creator, branchA, branchB, dispose };
            Model model = new Model(elements);
            model.Simulate(time: 1000.0, printEvents: false);
        }
    }
}
