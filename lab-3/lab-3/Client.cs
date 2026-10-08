using System;

namespace lab_3
{
    public class Client
    {
        private static int nextId = 0;
        public int Id { get; }
        public double ArrivalTime { get; set; }
        public int Type { get; set; }
        public int Priority { get; set; }

        public Client(double arrivalTime, int type = 1, int priority = 0)
        {
            Id = nextId++;
            ArrivalTime = arrivalTime;
            Type = type;
            Priority = priority;
        }
    }
}
