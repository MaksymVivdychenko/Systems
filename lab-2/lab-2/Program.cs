using System;
using System.Collections.Generic;

namespace lab_2
{
    public class SimModel
    {
        public static void Main(string[] args)
        {
            //Task1_2();
            //Task3();
            //Task4();
            //Task5();
            Task6();
        }
        
        public static void Task5()
        {
            Console.WriteLine("\n========================================================");
            Console.WriteLine("                        TASK 5                          ");
            Console.WriteLine("     Modeling with Multiple Identical Devices (Channels)");
            Console.WriteLine("========================================================");

            Create c = new Create(1.0);
            c.SetName("CREATOR");
            c.SetDistribution("exp");

            // Process with 3 channels and mean service delay of 2.0
            Process p = new Process(delay: 2.0, channels: 3);
            p.SetName("MULTICHANNEL_PROCESSOR");
            p.SetDistribution("exp");
            p.SetMaxqueue(5);

            Dispose d = new Dispose();
            d.SetName("DISPOSE");

            c.SetNextElement(p);
            p.SetNextElement(d);

            List<Element> list = new List<Element> { c, p, d };
            Model model = new Model(list);
            model.Simulate(1000.0);
        }
        
        public static void Task6()
        {
            Console.WriteLine("\n========================================================");
            Console.WriteLine("                        TASK 6                          ");
            Console.WriteLine("       Branching (Multiple Outputs) & Feedback Loop     ");
            Console.WriteLine("========================================================");

            Create c = new Create(2.0);
            c.SetName("CREATOR");
            c.SetDistribution("exp");

            Process p1 = new Process(delay: 1.0, channels: 2);
            p1.SetName("PROCESSOR 1 (Splitter)");
            p1.SetDistribution("exp");
            p1.SetMaxqueue(10);

            Process p2 = new Process(delay: 1.5, channels: 1);
            p2.SetName("PROCESSOR 2 (Inspection)");
            p2.SetDistribution("exp");
            p2.SetMaxqueue(5);

            Process p3 = new Process(delay: 1.2, channels: 1);
            p3.SetName("PROCESSOR 3 (Alt-Path)");
            p3.SetDistribution("exp");
            p3.SetMaxqueue(5);

            Dispose d = new Dispose();
            d.SetName("DISPOSE");

            // Connecting elements
            c.SetNextElement(p1);

            // Task 6.1: Branching into two subsequent blocks
            p1.AddNextElement(p2, 0.70);
            p1.AddNextElement(p3, 0.30);

            // Task 6.2: Feedback loop - return 15% back to previous block PROCESSOR 1!
            p2.AddNextElement(d, 0.85);
            p2.AddNextElement(p1, 0.15); // Loopback to previous block

            p3.AddNextElement(d, 1.00);

            List<Element> list = new List<Element> { c, p1, p2, p3, d };
            Model model = new Model(list);
            model.Simulate(1000.0);
        }
        
        public static void Task3()
        {
            Console.WriteLine("\n--- TASK 3 ---");
            Create c = new Create(2.0);
            Process p1 = new Process(1.0);
            Process p2 = new Process(1.0);
            Process p3 = new Process(1.0);
            Dispose d = new Dispose();

            c.SetName("CREATOR");
            p1.SetName("PROCESSOR 1");
            p2.SetName("PROCESSOR 2");
            p3.SetName("PROCESSOR 3");
            d.SetName("DISPOSE");

            c.SetDistribution("exp");
            p1.SetDistribution("exp");
            p2.SetDistribution("exp");
            p3.SetDistribution("exp");

            p1.SetMaxqueue(5);
            p2.SetMaxqueue(5);
            p3.SetMaxqueue(5);

            c.SetNextElement(p1);
            p1.SetNextElement(p2);
            p2.SetNextElement(p3);
            p3.SetNextElement(d);

            List<Element> list = new List<Element> { c, p1, p2, p3, d };
            Model model = new Model(list);
            model.Simulate(1000.0);
        }

        public static void Task4()
        {
            Console.WriteLine("\n========================================================");
            Console.WriteLine("                        TASK 4                          ");
            Console.WriteLine("       Model Verification with Parameter Variations     ");
            Console.WriteLine("========================================================");

            var scenarios = new[]
            {
                new { Name = "Exp 1: Baseline", DelayC = 2.0, DelayP1 = 1.0, DelayP2 = 1.0, DelayP3 = 1.0, MaxQ = 5 },
                new { Name = "Exp 2: Heavy Load", DelayC = 1.1, DelayP1 = 1.0, DelayP2 = 1.0, DelayP3 = 1.0, MaxQ = 5 },
                new { Name = "Exp 3: Bottleneck P2", DelayC = 2.0, DelayP1 = 1.0, DelayP2 = 2.2, DelayP3 = 1.0, MaxQ = 5 },
                new { Name = "Exp 4: Small Queue (Q=1)", DelayC = 1.5, DelayP1 = 1.3, DelayP2 = 1.3, DelayP3 = 1.3, MaxQ = 1 },
                new { Name = "Exp 5: Light Load", DelayC = 3.0, DelayP1 = 0.8, DelayP2 = 0.8, DelayP3 = 0.8, MaxQ = 5 }
            };

            Console.WriteLine($"\n{"Experiment",-24} | {"Arr/Serv Delays",-18} | {"MaxQ",-5} | {"Created",-7} | {"Dispose",-7} | {"TotalFail",-9} | {"Load P1/P2/P3",-20} | {"Queue P1/P2/P3",-18}");
            Console.WriteLine(new string('-', 125));

            foreach (var sc in scenarios)
            {
                Create c = new Create(sc.DelayC);
                Process p1 = new Process(sc.DelayP1);
                Process p2 = new Process(sc.DelayP2);
                Process p3 = new Process(sc.DelayP3);
                Dispose d = new Dispose();

                c.SetName("CREATOR");
                p1.SetName("P1");
                p2.SetName("P2");
                p3.SetName("P3");
                d.SetName("DISPOSE");

                c.SetDistribution("exp");
                p1.SetDistribution("exp");
                p2.SetDistribution("exp");
                p3.SetDistribution("exp");

                p1.SetMaxqueue(sc.MaxQ);
                p2.SetMaxqueue(sc.MaxQ);
                p3.SetMaxqueue(sc.MaxQ);

                c.SetNextElement(p1);
                p1.SetNextElement(p2);
                p2.SetNextElement(p3);
                p3.SetNextElement(d);

                Model model = new Model(new List<Element> { c, p1, p2, p3, d });
                model.Simulate(1000.0, printEvents: false);

                int totalFail = p1.GetFailure() + p2.GetFailure() + p3.GetFailure();
                string loads = $"{p1.GetMeanLoad() / 1000.0:F2} / {p2.GetMeanLoad() / 1000.0:F2} / {p3.GetMeanLoad() / 1000.0:F2}";
                string queues = $"{p1.GetMeanQueue() / 1000.0:F2} / {p2.GetMeanQueue() / 1000.0:F2} / {p3.GetMeanQueue() / 1000.0:F2}";

                Console.WriteLine($"{sc.Name,-24} | {sc.DelayC:F1}/({sc.DelayP1:F1},{sc.DelayP2:F1},{sc.DelayP3:F1}) | {sc.MaxQ,-5} | {c.GetQuantity(),-7} | {d.GetQuantity(),-7} | {totalFail,-9} | {loads,-20} | {queues,-18}");
            }
            Console.WriteLine(new string('-', 125));
        }

        public static void Task1_2()
        {
            Console.WriteLine("\n--- TASKS 1 & 2 ---");
            Create c = new Create(2.0);
            Process p = new Process(1.0);
            Console.WriteLine("id0 = " + c.GetId() + " id1=" + p.GetId());
            c.SetNextElement(p);
            p.SetMaxqueue(5);
            c.SetName("CREATOR");
            p.SetName("PROCESSOR");
            c.SetDistribution("exp");
            p.SetDistribution("exp");

            List<Element> list = new List<Element>();
            list.Add(c);
            list.Add(p);
            Model model = new Model(list);
            model.Simulate(1000.0);
        }
    }
}