using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Doan1.Models.StatePattern
{
    public class ContextState
    {
        private State state;


        public void setState(State state)
        {
            this.state = state;
        }

        public void applyState(string iddonhang)
        {
            this.state.ChangeState(iddonhang);
        }
    }
}