using System;
using System.Collections.Generic;

namespace lab_2
{
    public class Model
    {
        private List<Element> list = new List<Element>();
        double tnext, tcurr;
        int eventId;

        public Model(List<Element> elements)
        {
            list = elements;
            tnext = 0.0;
            eventId = 0;
            tcurr = tnext;
        }

        public void Simulate(double time, bool printEvents = true)
        {
            while (tcurr < time)
            {
                tnext = Double.MaxValue;
                foreach (Element e in list)
                {
                    if (e.GetTnext() < tnext)
                    {
                        tnext = e.GetTnext();
                        eventId = e.GetId();
                    }
                }
                
                int eventIndex = list.FindIndex(e => e.GetId() == eventId);
                if (eventIndex == -1) eventIndex = 0;

                if (printEvents)
                {
                    Console.WriteLine("\nIt's time for event in " +
                        list[eventIndex].GetName() +
                        ", time = " + tnext);
                }
                
                foreach (Element e in list)
                {
                    e.DoStatistics(tnext - tcurr);
                }
                tcurr = tnext;
                foreach (Element e in list)
                {
                    e.SetTcurr(tcurr);
                }
                
                list[eventIndex].OutAct();
                foreach (Element e in list)
                {
                    if (e.GetTnext() == tcurr)
                    {
                        e.OutAct();
                    }
                }
                if (printEvents)
                {
                    PrintInfo();
                }
            }
            PrintResult();
        }

        public void PrintInfo()
        {
            foreach (Element e in list)
            {
                e.PrintInfo();
            }
        }

        public void PrintResult()
        {
            Console.WriteLine("\n-------------RESULTS-------------");
            foreach (Element e in list)
            {
                e.PrintResult();
                if (e is Process)
                {
                    Process p = (Process)e;
                    double failProb = (p.GetQuantity() + p.GetFailure()) > 0
                        ? p.GetFailure() / (double)(p.GetQuantity() + p.GetFailure())
                        : 0.0;

                    Console.WriteLine("mean length of queue = " + p.GetMeanQueue() / tcurr +
                        "\nfailure probability  = " + failProb +
                        "\naverage device load  = " + (p.GetMeanLoad() / tcurr) / p.GetChannels());

                    if (p.GetChannels() > 1)
                    {
                        Console.WriteLine($"mean busy devices    = {p.GetMeanLoad() / tcurr:F4} of {p.GetChannels()} channels");
                    }
                    p.PrintRoutes();
                }
            }
        }
    }
}
