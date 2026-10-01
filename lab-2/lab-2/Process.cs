using System;
using System.Collections.Generic;

namespace lab_2
{
    public class Process : Element
    {
        private int queue, maxqueue, failure;
        private double meanQueue;
        private double meanLoad;

        // Multichannel support (Task 5)
        private int channels;
        private int[] channelStates;
        private double[] tnextChannels;

        // Multi-route / Branching support (Task 6)
        private List<Route> nextRoutes = new List<Route>();

        public Process(double delay) : this(delay, 1)
        {
        }

        public Process(double delay, int channels) : base(delay)
        {
            queue = 0;
            maxqueue = int.MaxValue;
            meanQueue = 0.0;
            meanLoad = 0.0;
            this.channels = channels > 0 ? channels : 1;
            InitChannels();
        }

        private void InitChannels()
        {
            channelStates = new int[channels];
            tnextChannels = new double[channels];
            for (int i = 0; i < channels; i++)
            {
                channelStates[i] = 0;
                tnextChannels[i] = Double.MaxValue;
            }
            base.SetTnext(Double.MaxValue);
            base.SetState(0);
        }

        public int GetChannels()
        {
            return channels;
        }

        public void SetChannels(int count)
        {
            this.channels = count > 0 ? count : 1;
            InitChannels();
        }

        public int GetBusyChannelsCount()
        {
            int count = 0;
            for (int i = 0; i < channels; i++)
            {
                if (channelStates[i] == 1)
                {
                    count++;
                }
            }
            return count;
        }

        private int GetFreeChannelIndex()
        {
            for (int i = 0; i < channels; i++)
            {
                if (channelStates[i] == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        private int GetEarliestFinishedChannelIndex()
        {
            int minIdx = -1;
            double minT = Double.MaxValue;
            for (int i = 0; i < channels; i++)
            {
                if (channelStates[i] == 1 && tnextChannels[i] < minT)
                {
                    minT = tnextChannels[i];
                    minIdx = i;
                }
            }
            return minIdx;
        }

        private void UpdateProcessTnext()
        {
            double minT = Double.MaxValue;
            for (int i = 0; i < channels; i++)
            {
                if (channelStates[i] == 1 && tnextChannels[i] < minT)
                {
                    minT = tnextChannels[i];
                }
            }
            base.SetTnext(minT);
            base.SetState(GetBusyChannelsCount());
        }

        public override void InAct()
        {
            int freeChannel = GetFreeChannelIndex();
            if (freeChannel != -1)
            {
                channelStates[freeChannel] = 1;
                tnextChannels[freeChannel] = base.GetTcurr() + base.GetDelay();
                UpdateProcessTnext();
            }
            else
            {
                if (GetQueue() < GetMaxqueue())
                {
                    SetQueue(GetQueue() + 1);
                }
                else
                {
                    failure++;
                }
            }
        }

        public override void OutAct()
        {
            base.OutAct();

            int finishedChannel = GetEarliestFinishedChannelIndex();
            if (finishedChannel != -1)
            {
                tnextChannels[finishedChannel] = Double.MaxValue;
                channelStates[finishedChannel] = 0;

                if (GetQueue() > 0)
                {
                    SetQueue(GetQueue() - 1);
                    channelStates[finishedChannel] = 1;
                    tnextChannels[finishedChannel] = base.GetTcurr() + base.GetDelay();
                }
            }

            UpdateProcessTnext();

            RouteToNext();
        }

        public void AddNextElement(Element element, double probability)
        {
            nextRoutes.Add(new Route(element, probability));
        }

        public List<Route> GetRoutes()
        {
            return nextRoutes;
        }

        private void RouteToNext()
        {
            if (nextRoutes.Count > 0)
            {
                double rand = FunRand.Unif(0, 1);
                double cumulative = 0.0;
                Route selectedRoute = null;

                foreach (var r in nextRoutes)
                {
                    cumulative += r.Probability;
                    if (rand <= cumulative)
                    {
                        selectedRoute = r;
                        break;
                    }
                }

                if (selectedRoute == null)
                {
                    selectedRoute = nextRoutes[nextRoutes.Count - 1];
                }

                selectedRoute.TransitionCount++;
                selectedRoute.NextElement.InAct();
            }
            else if (base.GetNextElement() != null)
            {
                base.GetNextElement().InAct();
            }
        }

        public int GetFailure()
        {
            return failure;
        }

        public int GetQueue()
        {
            return queue;
        }

        public void SetQueue(int queue)
        {
            this.queue = queue;
        }

        public int GetMaxqueue()
        {
            return maxqueue;
        }

        public void SetMaxqueue(int maxqueue)
        {
            this.maxqueue = maxqueue;
        }

        public override void PrintInfo()
        {
            base.PrintInfo();
            Console.WriteLine($"failure = {this.GetFailure()}, busy channels = {GetBusyChannelsCount()}/{channels}");
        }

        public override void DoStatistics(double delta)
        {
            meanQueue = GetMeanQueue() + queue * delta;
            meanLoad = meanLoad + GetBusyChannelsCount() * delta;
        }

        public double GetMeanQueue()
        {
            return meanQueue;
        }

        public double GetMeanLoad()
        {
            return meanLoad;
        }

        public void PrintRoutes()
        {
            if (nextRoutes.Count > 1)
            {
                Console.WriteLine($"Routes from {GetName()}:");
                foreach (var r in nextRoutes)
                {
                    double actualPct = GetQuantity() > 0 ? (r.TransitionCount / (double)GetQuantity()) * 100.0 : 0.0;
                    Console.WriteLine($"  -> {r.NextElement.GetName()}: prob={r.Probability:F2} (actual transitions: {r.TransitionCount}, {actualPct:F1}%)");
                }
            }
        }
    }
}
