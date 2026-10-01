using System;

namespace lab_2
{
    public class Create : Element
    {
        public Create(double delay) : base(delay)
        {
            base.SetTnext(0.0); // імітація розпочнеться з події Create
        }

        public override void OutAct()
        {
            base.OutAct();
            base.SetTnext(base.GetTcurr() + base.GetDelay());
            if (base.GetNextElement() != null)
            {
                base.GetNextElement().InAct();
            }
        }
    }
}
