using System;
using System.Collections.Generic;
using System.Linq;

namespace lab_3
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
        private Element? nextElement;
        private static int nextId = 0;
        private int id;

        // Unified routing support
        protected List<Route> nextRoutes = new List<Route>();
        private RouteMode routeMode = RouteMode.ByProbability;

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
            else if ("erlang".Equals(GetDistribution(), StringComparison.OrdinalIgnoreCase))
            {
                delay = FunRand.Erlang(GetDelayMean(), (int)GetDelayDev());
            }
            else if ("".Equals(GetDistribution(), StringComparison.OrdinalIgnoreCase))
            {
                delay = GetDelayMean();
            }
            return delay;
        }

        public virtual double GetDelayForClient(Client? client)
        {
            return GetDelay();
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

        public Element? GetNextElement()
        {
            return nextElement;
        }

        public void SetNextElement(Element? nextElement)
        {
            this.nextElement = nextElement;
        }

        #region Unified Routing

        public void AddNextElement(Element element, double probability = 1.0, int priority = 1)
        {
            nextRoutes.Add(new Route(element, probability, priority));
        }

        public void AddNextElement(Element element, int priority)
        {
            nextRoutes.Add(new Route(element, probability: 1.0, priority: priority));
        }

        public void AddNextElement(Route route)
        {
            nextRoutes.Add(route);
        }

        public List<Route> GetRoutes()
        {
            return nextRoutes;
        }

        public RouteMode GetRouteMode()
        {
            return routeMode;
        }

        public void SetRouteMode(RouteMode mode)
        {
            this.routeMode = mode;
        }

        public virtual bool CanAccept()
        {
            return true;
        }

        public virtual int GetCurrentQueue()
        {
            return 0;
        }

        public virtual bool HasFreeChannel()
        {
            return true;
        }

        protected virtual void RouteToNext(Client? client = null)
        {
            if (nextRoutes.Count > 0)
            {
                Route? selectedRoute = SelectNextRoute();
                if (selectedRoute != null)
                {
                    selectedRoute.TransitionCount++;
                    selectedRoute.NextElement.InAct(client);
                }
            }
            else if (GetNextElement() is Element next)
            {
                next.InAct(client);
            }
        }

        protected virtual Route? SelectNextRoute()
        {
            if (nextRoutes.Count == 0) return null;
            if (nextRoutes.Count == 1) return nextRoutes[0];

            return routeMode switch
            {
                RouteMode.ByPriority => SelectRouteByPriority(),
                RouteMode.ByQueuePriority => SelectRouteByQueuePriority(),
                RouteMode.ByProbability => SelectRouteByProbability(),
                _ => SelectRouteByProbability()
            };
        }

        private Route? SelectRouteByProbability()
        {
            double rand = FunRand.Unif(0, 1);
            double cumulative = 0.0;
            Route? selected = null;

            foreach (var r in nextRoutes)
            {
                cumulative += r.Probability;
                if (rand <= cumulative)
                {
                    selected = r;
                    break;
                }
            }

            return selected ?? nextRoutes[nextRoutes.Count - 1];
        }

        private Route? SelectRouteByPriority()
        {
            // Сортування за пріоритетом: менше число = вищий пріоритет (1 > 2 > 3)
            var sorted = nextRoutes.OrderBy(r => r.Priority).ToList();

            foreach (var r in sorted)
            {
                if (r.NextElement.CanAccept())
                {
                    return r;
                }
            }

            // Якщо всі зайняті/заповнені — обираємо маршрут із найвищим пріоритетом (відмова)
            return sorted[0];
        }

        private Route? SelectRouteByQueuePriority()
        {
            // Кандидати, які можуть прийняти вимогу
            var candidates = nextRoutes.Where(r => r.NextElement.CanAccept()).ToList();
            if (candidates.Count == 0)
            {
                candidates = nextRoutes;
            }

            Route? best = null;
            int minQueue = int.MaxValue;
            int bestPriority = int.MaxValue;
            bool bestHasFreeChannel = false;

            foreach (var r in candidates)
            {
                int q = r.NextElement.GetCurrentQueue();
                int p = r.Priority;
                bool freeChan = r.NextElement.HasFreeChannel();

                bool isBetter = false;
                if (best == null)
                {
                    isBetter = true;
                }
                else if (freeChan && !bestHasFreeChannel)
                {
                    isBetter = true; // Вільний канал має вищий пріоритет над чергою
                }
                else if (!freeChan && bestHasFreeChannel)
                {
                    isBetter = false;
                }
                else
                {
                    if (q < minQueue)
                    {
                        isBetter = true;
                    }
                    else if (q == minQueue && p < bestPriority)
                    {
                        isBetter = true;
                    }
                }

                if (isBetter)
                {
                    best = r;
                    minQueue = q;
                    bestPriority = p;
                    bestHasFreeChannel = freeChan;
                }
            }

            return best ?? candidates[0];
        }

        public virtual void PrintRoutes()
        {
            if (nextRoutes.Count > 1)
            {
                Console.WriteLine($"Routes from {GetName()} (Mode: {routeMode}):");
                int totalTransitions = nextRoutes.Sum(r => r.TransitionCount);
                foreach (var r in nextRoutes)
                {
                    double actualPct = totalTransitions > 0 ? (r.TransitionCount / (double)totalTransitions) * 100.0 : 0.0;
                    if (routeMode == RouteMode.ByProbability)
                    {
                        Console.WriteLine($"  -> {r.NextElement.GetName()}: prob={r.Probability:F2}, priority={r.Priority} (actual: {r.TransitionCount}, {actualPct:F1}%)");
                    }
                    else
                    {
                        Console.WriteLine($"  -> {r.NextElement.GetName()}: priority={r.Priority}, queue={r.NextElement.GetCurrentQueue()} (actual: {r.TransitionCount}, {actualPct:F1}%)");
                    }
                }
            }
        }

        #endregion

        public virtual void InAct(Client? client = null)
        {
        }

        public virtual void OutAct(Client? client = null)
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
