using System;

namespace lab_3
{
    public enum RouteMode
    {
        ByProbability,   // Вибір за ймовірністю (стохастичний)
        ByPriority,      // Вибір за пріоритетом (найвищий пріоритет серед доступних)
        ByQueuePriority  // Вибір черги за пріоритетом (найкоротша черга, при рівності — вищий пріоритет)
    }

    public class Route
    {
        public Element NextElement { get; set; }
        public double Probability { get; set; }
        public int Priority { get; set; } // Менше значення = вищий пріоритет (1 > 2 > 3)
        public int TransitionCount { get; set; }

        public Route(Element nextElement, double probability = 1.0, int priority = 1)
        {
            NextElement = nextElement;
            Probability = probability;
            Priority = priority;
            TransitionCount = 0;
        }

        public Route(Element nextElement, int priority) : this(nextElement, 1.0, priority)
        {
        }
    }
}
