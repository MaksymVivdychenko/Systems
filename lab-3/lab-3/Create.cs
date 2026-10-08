using System;

namespace lab_3
{
    public class Create : Element
    {
        public Create(double delay) : base(delay)
        {
            base.SetTnext(0.0); // імітація розпочнеться з події Create
        }

        protected virtual Client GenerateClient()
        {
            return new Client(base.GetTcurr());
        }

        public override void OutAct(Client? client = null)
        {
            base.OutAct(client);
            base.SetTnext(base.GetTcurr() + base.GetDelay());
            
            Client newClient = GenerateClient();
            RouteToNext(newClient);
        }
    }
}
