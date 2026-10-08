using System;

namespace lab_3.Task3
{
    public class LabRegistryProcess : Process
    {
        private double lastArrivalTime = 0.0;
        private double sumArrivalIntervals = 0.0;
        private int arrivalsCount = 0;

        public LabRegistryProcess(double delay, int channels) : base(delay, channels)
        {
        }

        public override void InAct(Client? client = null)
        {
            if (arrivalsCount > 0)
            {
                sumArrivalIntervals += (GetTcurr() - lastArrivalTime);
            }
            lastArrivalTime = GetTcurr();
            arrivalsCount++;
            
            base.InAct(client);
        }

        public double GetAverageArrivalInterval()
        {
            return arrivalsCount > 1 ? sumArrivalIntervals / (arrivalsCount - 1) : 0.0;
        }
    }
}
