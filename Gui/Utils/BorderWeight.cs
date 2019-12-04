namespace Foster.GuiSystem
{
    public struct BorderWeight
    {
        public float Left;
        public float Top;
        public float Right;
        public float Bottom;

        public float Width => Left + Right;
        public float Height => Top + Bottom;
        public bool Weighted => Left > 0 || Top > 0 || Right > 0 || Bottom > 0;

        public BorderWeight(float left, float top, float right, float bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public BorderWeight(float radius)
        {
            Left = radius;
            Top = radius;
            Right = radius;
            Bottom = radius;
        }

        public static implicit operator BorderWeight(float radius) => new BorderWeight(radius);
    }
}
