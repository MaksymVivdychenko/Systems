using System;

namespace lab_3
{
    public class Dispose : Element
    {
        private double totalTimeInSystem = 0.0;
        private int processedCount = 0;
        private double lastDepartureTime = 0.0;
        private double sumDepartureIntervals = 0.0;
        private int departuresCount = 0;

        public Dispose() : base()
        {
            SetName("DISPOSE");
        }

        public override void InAct(Client? client = null)
        {
            base.OutAct(client);

            if (client != null)
            {
                totalTimeInSystem += (GetTcurr() - client.ArrivalTime);
                processedCount++;
            }

            if (departuresCount > 0)
            {
                sumDepartureIntervals += (GetTcurr() - lastDepartureTime);
            }
            
            lastDepartureTime = GetTcurr();
            departuresCount++;
        }

        public double GetAverageTimeInSystem()
        {
            return processedCount > 0 ? totalTimeInSystem / processedCount : 0.0;
        }

        public double GetAverageDepartureInterval()
        {
            return departuresCount > 1 ? sumDepartureIntervals / (departuresCount - 1) : 0.0;
        }
    }
}
