using System;

namespace lab_3.Task3
{
    public class PatientCreator : Create
    {
        public PatientCreator(double delay) : base(delay)
        {
        }

        protected override Client GenerateClient()
        {
            double rand = FunRand.GetNextDouble();
            int type = 3;
            if (rand < 0.5)
            {
                type = 1;
            }
            else if (rand < 0.6)
            {
                type = 2;
            }

            int priority = (type == 1) ? 1 : 2;
            return new Client(base.GetTcurr(), type, priority);
        }
    }
}
