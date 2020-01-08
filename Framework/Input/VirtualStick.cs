namespace Foster.Framework
{
    /// <summary>
    /// A Virtual Input Stick that can be mapped to different keyboards and gamepads
    /// </summary>
    public class VirtualStick
    {

        public VirtualAxis Horizontal;
        public VirtualAxis Vertical;

        public Vector2 Value => new Vector2(Horizontal.Value, Vertical.Value);

        public Vector2 ValueNoDeadzone => new Vector2(Horizontal.ValueNoDeadzone, Vertical.ValueNoDeadzone);

        public Point2 IntValue => new Point2(Horizontal.IntValue, Vertical.IntValue);

        public Point2 IntValueNoDeadzone => new Point2(Horizontal.IntValueNoDeadzone, Vertical.IntValueNoDeadzone);

        public VirtualStick(Input input)
        {
            Horizontal = new VirtualAxis(input);
            Vertical = new VirtualAxis(input);
        }

        public VirtualStick(Input input, VirtualAxis.Overlaps overlapBehaviour)
        {
            Horizontal = new VirtualAxis(input, overlapBehaviour);
            Vertical = new VirtualAxis(input, overlapBehaviour);
        }

        public VirtualStick Add(Keys left, Keys right, Keys up, Keys down)
        {
            Horizontal.Add(left, right);
            Vertical.Add(up, down);
            return this;
        }

        public VirtualStick Add(int controller, Buttons left, Buttons right, Buttons up, Buttons down)
        {
            Horizontal.Add(controller, left, right);
            Vertical.Add(controller, up, down);
            return this;
        }

        public VirtualStick AddLeftJoystick(int controller, float deadzoneX = 0, float deadzoneY = 0)
        {
            Horizontal.Add(controller, Axes.LeftX, deadzoneX);
            Vertical.Add(controller, Axes.LeftY, deadzoneY);
            return this;
        }

        public VirtualStick AddRightJoystick(int controller, float deadzoneX = 0, float deadzoneY = 0)
        {
            Horizontal.Add(controller, Axes.RightX, deadzoneX);
            Vertical.Add(controller, Axes.RightY, deadzoneY);
            return this;
        }

    }
}
