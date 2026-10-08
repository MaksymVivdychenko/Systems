using System;

namespace lab_3.Task3
{
    public class LabAnalysisProcess : Process
    {
        public LabAnalysisProcess(double delay, int channels) : base(delay, channels)
        {
        }

        protected override void RouteToNext(Client? client = null)
        {
            if (client != null && nextRoutes.Count >= 2)
            {
                if (client.Type == 2)
                {
                    // Type 2 returns to Reception and becomes Type 1
                    client.Type = 1;
                    client.Priority = 1;
                    nextRoutes[0].TransitionCount++;
                    nextRoutes[0].NextElement.InAct(client); // WalkToReception
                }
                else if (client.Type == 3)
                {
                    // Type 3 leaves the hospital
                    nextRoutes[1].TransitionCount++;
                    nextRoutes[1].NextElement.InAct(client); // DisposeExit
                }
            }
        }
    }
}
