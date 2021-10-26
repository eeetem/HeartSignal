using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeartSignal
{
    public interface ITextInputReciver
    {
        void ReciveInput(string text);
        public InputConsole GetInputSource();
    }
}
