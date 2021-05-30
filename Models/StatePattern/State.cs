using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Doan1.Models.StatePattern
{
     abstract public class State
    {
        public abstract void ChangeState(string iddonhang);

    }
}