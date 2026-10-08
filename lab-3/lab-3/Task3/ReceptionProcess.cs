using System;

namespace lab_3.Task3
{
    public class ReceptionProcess : Process
    {
        public ReceptionProcess(int channels) : base(0.0, channels)
        {
        }

        public override double GetDelayForClient(Client? client)
        {
            if (client != null)
            {
                if (client.Type == 1) return FunRand.Exp(15.0);
                if (client.Type == 2) return FunRand.Exp(40.0);
                if (client.Type == 3) return FunRand.Exp(30.0);
            }
            return base.GetDelay();
        }

        protected override void RouteToNext(Client? client = null)
        {
            if (client != null && nextRoutes.Count >= 2)
            {
                if (client.Type == 1)
                {
                    // Type 1 -> Escort to Ward (assumed first route)
                    nextRoutes[0].TransitionCount++;
                    nextRoutes[0].NextElement.InAct(client);
                }
                else
                {
                    // Type 2, 3 -> Walk to Lab (assumed second route)
                    nextRoutes[1].TransitionCount++;
                    nextRoutes[1].NextElement.InAct(client);
                }
            }
        }
    }
}
