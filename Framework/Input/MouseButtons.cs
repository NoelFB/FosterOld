using System.Collections.Generic;
using System.Collections;
namespace Foster.Framework
{
    /// <summary>
    /// Mouse Buttons
    /// </summary>
    public enum MouseButtons
    {
        None = 0,
        Unknown = 1,
        Left = 2,
        Middle = 3,
        Right = 4
    }

    public static class MouseButtonsExt
    {
        public static IEnumerable<MouseButtons> All
        {
            get
            {
                yield return MouseButtons.Left;
                yield return MouseButtons.Middle;
                yield return MouseButtons.Right;
            }
        }
    }
}
