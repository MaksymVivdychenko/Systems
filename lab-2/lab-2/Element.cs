using System;

namespace lab_2
{
    public class Element
    {
        private string name;
        private double tnext;
        private double delayMean, delayDev;
        private string distribution;
        private int quantity;
        private double tcurr;
        private int state;
        private Element nextElement;
        private static int nextId = 0;
        private int id;

        public Element()
        {
            tnext = Double.MaxValue;
            delayMean = 1.0;
            distribution = "exp";
            tcurr = tnext;
            state = 0;
            nextElement = null;
            id = nextId;
            nextId++;
            name = "element" + id;
        }

        public Element(double delay)
        {
            name = "anonymus";
            tnext = 0.0;
            delayMean = delay;
            distribution = "";
            tcurr = tnext;
            state = 0;
            nextElement = null;
            id = nextId;
            nextId++;
            name = "element" + id;
        }

        public Element(string nameOfElement, double delay)
        {
            name = nameOfElement;
            tnext = 0.0;
            delayMean = delay;
            distribution = "exp";
            tcurr = tnext;
            state = 0;
            nextElement = null;
            id = nextId;
            nextId++;
            name = "element" + id;
        }

        public double GetDelay()
        {
            double delay = GetDelayMean();
            if ("exp".Equals(GetDistribution(), StringComparison.OrdinalIgnoreCase))
            {
                delay = FunRand.Exp(GetDelayMean());
            }
            else if ("norm".Equals(GetDistribution(), StringComparison.OrdinalIgnoreCase))
            {
                delay = FunRand.Norm(GetDelayMean(), GetDelayDev());
            }
            else if ("unif".Equals(GetDistribution(), StringComparison.OrdinalIgnoreCase))
            {
                delay = FunRand.Unif(GetDelayMean(), GetDelayDev());
            }
            else if ("".Equals(GetDistribution(), StringComparison.OrdinalIgnoreCase))
            {
                delay = GetDelayMean();
            }
            return delay;
        }

        public double GetDelayDev()
        {
            return delayDev;
        }

        public void SetDelayDev(double delayDev)
        {
            this.delayDev = delayDev;
        }

        public string GetDistribution()
        {
            return distribution;
        }

        public void SetDistribution(string distribution)
        {
            this.distribution = distribution;
        }

        public int GetQuantity()
        {
            return quantity;
        }

        public double GetTcurr()
        {
            return tcurr;
        }

        public void SetTcurr(double tcurr)
        {
            this.tcurr = tcurr;
        }

        public int GetState()
        {
            return state;
        }

        public void SetState(int state)
        {
            this.state = state;
        }

        public Element GetNextElement()
        {
            return nextElement;
        }

        public void SetNextElement(Element nextElement)
        {
            this.nextElement = nextElement;
        }

        public virtual void InAct()
        {
        }

        public virtual void OutAct()
        {
            quantity++;
        }

        public double GetTnext()
        {
            return tnext;
        }

        public void SetTnext(double tnext)
        {
            this.tnext = tnext;
        }

        public double GetDelayMean()
        {
            return delayMean;
        }

        public void SetDelayMean(double delayMean)
        {
            this.delayMean = delayMean;
        }

        public int GetId()
        {
            return id;
        }

        public void SetId(int id)
        {
            this.id = id;
        }

        public virtual void PrintResult()
        {
            Console.WriteLine(GetName() + " quantity = " + quantity);
        }

        public virtual void PrintInfo()
        {
            Console.WriteLine(GetName() + " state= " + state +
                " quantity = " + quantity +
                " tnext= " + tnext);
        }

        public string GetName()
        {
            return name;
        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public virtual void DoStatistics(double delta)
        {
        }
    }
}
