﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeartSignal
{
    interface ITextInputReciver
    {
        void ReciveInput(string text);
        InputConsole GetInputSource();
    }
}
