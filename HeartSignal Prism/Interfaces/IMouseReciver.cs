using System;
using System.Collections.Generic;
using System.Linq;
using SadConsole;
using Console = SadConsole.Console;
using SadRogue.Primitives;

namespace HeartSignal
{
    interface IMouseReciver
    {
        void MousePosUpdate(Point pos);
    }
}
