using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;

namespace MapNotify
{
    class MouseLite
    {
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point lpPoint);

        public static Point GetCursorPosition()
        {
            GetCursorPos(out var lpPoint);
            return lpPoint;
        }
        public static Vector2 GetCursorPositionVector()
        {
            var currentMousePoint = GetCursorPosition();
            return new Vector2(currentMousePoint.X, currentMousePoint.Y);
        }
    }
}
