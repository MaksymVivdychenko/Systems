using System;

namespace lab_2
{
    public class Route
    {
        public Element NextElement { get; set; }
        public double Probability { get; set; }
        public int TransitionCount { get; set; }

        public Route(Element nextElement, double probability)
        {
            NextElement = nextElement;
            Probability = probability;
            TransitionCount = 0;
        }
    }
}
