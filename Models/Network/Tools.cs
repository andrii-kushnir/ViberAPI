using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models.Network
{
    public static class EventExtensions
    {
        public static void Raise<T>(this EventHandler<T> handler, Object sender, T args)
        {
            if (handler != null) handler(sender, args);
        }

        public static void Raise(this EventHandler handler, Object sender, EventArgs args)
        {
            if (handler != null) handler(sender, args);
        }
    }
}
