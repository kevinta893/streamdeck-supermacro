using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;

namespace SuperMacro.Backend
{
    internal static class MouseHandler
    {
        public static bool HandleMouseMacro(InputSimulator iis, VirtualKeyCode keyCode)
        {
            bool handled = false;

            // Try handling mouse
            switch (keyCode)
            {
                case VirtualKeyCode.LBUTTON:
                    iis.Mouse.LeftButtonClick();
                    handled = true;
                    break;
                case VirtualKeyCode.RBUTTON:
                    iis.Mouse.RightButtonClick();
                    handled = true;
                    break;
                case VirtualKeyCode.MBUTTON:
                    iis.Mouse.MiddleButtonClick();
                    handled = true;
                    break;
                case VirtualKeyCode.XBUTTON1:
                    iis.Mouse.XButtonClick(1);
                    handled = true;
                    break;
                case VirtualKeyCode.XBUTTON2:
                    iis.Mouse.XButtonClick(2);
                    handled = true;
                    break;
            }
            return handled;
        }
    }
}
