using SadConsole.Input;
using SadRogue.Primitives;

namespace HeartSignal
{
    interface IMouseInputReciver
    {
        void Clicked(Point clickloc, MouseScreenObjectState state);
        void RightClicked(Point clickloc, MouseScreenObjectState state);
    }
}
