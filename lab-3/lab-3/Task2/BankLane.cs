using System;
using System.Collections.Generic;

namespace lab_3
{
    public class BankLane : Process
    {
        public BankLane? AdjacentLane { get; set; }
        public int LaneSwitchesCount { get; private set; }

        public BankLane(double delay, int channels = 1) : base(delay, channels)
        {
        }

        public void SetInitialBankState(double initialDelayMean, double initialDelayDev, int initialQueueSize)
        {
            if (channels > 0)
            {
                channelClients[0] = new Client(0.0);
                tnextChannels[0] = FunRand.Norm(initialDelayMean, initialDelayDev);
            }
            
            for (int i = 0; i < initialQueueSize; i++)
            {
                clientQueue.Add(new Client(0.0));
            }
            
            UpdateProcessTnext();
        }

        public override void OutAct(Client? client = null)
        {
            base.OutAct(client);
            CheckLaneSwitching();
        }

        private void CheckLaneSwitching()
        {
            if (AdjacentLane != null)
            {
                int diff = AdjacentLane.GetQueue() - this.GetQueue();
                if (diff >= 2)
                {
                    Client? carToMove = AdjacentLane.RemoveLastClient();
                    if (carToMove != null)
                    {
                        this.clientQueue.Add(carToMove);
                        LaneSwitchesCount++;
                    }
                }
            }
        }

        public Client? RemoveLastClient()
        {
            if (clientQueue.Count > 0)
            {
                Client last = clientQueue[clientQueue.Count - 1];
                clientQueue.RemoveAt(clientQueue.Count - 1);
                return last;
            }
            return null;
        }
    }
}
