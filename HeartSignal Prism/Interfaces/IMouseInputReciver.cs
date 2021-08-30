using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SadConsole;
using SadRogue.Primitives;

namespace HeartSignal
{
    interface IMouseInputReciver
    {
        void Clicked(Point clickloc);
    }
}
