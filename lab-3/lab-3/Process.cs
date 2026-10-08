using System;
using System.Collections.Generic;

namespace lab_3
{
    public class Process : Element
    {
        protected List<Client> clientQueue = new List<Client>();
        private int maxqueue, failure;
        private double meanQueue;
        private double meanLoad;

        // Multichannel support
        protected int channels;
        protected double[] tnextChannels = Array.Empty<double>();
        protected Client?[] channelClients = Array.Empty<Client>();

        public Process(double delay) : this(delay, 1)
        {
        }

        public Process(double delay, int channels) : base(delay)
        {
            maxqueue = int.MaxValue;
            meanQueue = 0.0;
            meanLoad = 0.0;
            this.channels = channels > 0 ? channels : 1;
            InitChannels();
        }

        private void InitChannels()
        {
            tnextChannels = new double[channels];
            channelClients = new Client?[channels];
            for (int i = 0; i < channels; i++)
            {
                tnextChannels[i] = Double.MaxValue;
                channelClients[i] = null;
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
                if (channelClients[i] != null)
                {
                    count++;
                }
            }
            return count;
        }

        public override bool HasFreeChannel()
        {
            return GetBusyChannelsCount() < channels;
        }

        public override bool CanAccept()
        {
            return HasFreeChannel() || GetQueue() < maxqueue;
        }

        public override int GetCurrentQueue()
        {
            return GetQueue();
        }

        protected int GetFreeChannelIndex()
        {
            for (int i = 0; i < channels; i++)
            {
                if (channelClients[i] == null)
                {
                    return i;
                }
            }
            return -1;
        }

        protected int GetEarliestFinishedChannelIndex()
        {
            int minIdx = -1;
            double minT = Double.MaxValue;
            for (int i = 0; i < channels; i++)
            {
                if (channelClients[i] != null && tnextChannels[i] < minT)
                {
                    minT = tnextChannels[i];
                    minIdx = i;
                }
            }
            return minIdx;
        }

        protected void UpdateProcessTnext()
        {
            double minT = Double.MaxValue;
            for (int i = 0; i < channels; i++)
            {
                if (channelClients[i] != null && tnextChannels[i] < minT)
                {
                    minT = tnextChannels[i];
                }
            }
            base.SetTnext(minT);
            base.SetState(GetBusyChannelsCount());
        }

        public override void InAct(Client? client = null)
        {
            int freeChannel = GetFreeChannelIndex();
            Client newClient = client ?? new Client(base.GetTcurr());

            if (freeChannel != -1)
            {
                channelClients[freeChannel] = newClient;
                tnextChannels[freeChannel] = base.GetTcurr() + GetDelayForClient(newClient);
                UpdateProcessTnext();
            }
            else
            {
                if (GetQueue() < GetMaxqueue())
                {
                    int insertIndex = clientQueue.Count;
                    for (int i = 0; i < clientQueue.Count; i++)
                    {
                        if (clientQueue[i].Priority > newClient.Priority)
                        {
                            insertIndex = i;
                            break;
                        }
                    }
                    clientQueue.Insert(insertIndex, newClient);
                }
                else
                {
                    failure++;
                }
            }
        }

        public override void OutAct(Client? client = null)
        {
            int finishedChannel = GetEarliestFinishedChannelIndex();
            Client? departingClient = null;

            if (finishedChannel != -1)
            {
                departingClient = channelClients[finishedChannel];
                tnextChannels[finishedChannel] = Double.MaxValue;
                channelClients[finishedChannel] = null;

                if (GetQueue() > 0)
                {
                    Client nextClient = clientQueue[0];
                    clientQueue.RemoveAt(0);
                    channelClients[finishedChannel] = nextClient;
                    tnextChannels[finishedChannel] = base.GetTcurr() + GetDelayForClient(nextClient);
                }
                
                base.OutAct(departingClient); // Increment quantity here
            }

            UpdateProcessTnext();

            RouteToNext(departingClient);
        }

        public int GetFailure()
        {
            return failure;
        }

        public int GetQueue()
        {
            return clientQueue.Count;
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
            meanQueue = GetMeanQueue() + GetQueue() * delta;
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
    }
}
