namespace Foster.GUI
{
    public struct BorderRadius
    {
        public float TopLeft;
        public float TopRight;
        public float BottomRight;
        public float BottomLeft;

        public bool Rounded => TopLeft > 0 || TopRight > 0 || BottomRight > 0 || BottomLeft > 0;

        public BorderRadius(float topLeft, float topRight, float bottomRight, float bottomLeft)
        {
            TopLeft = topLeft;
            TopRight = topRight;
            BottomRight = bottomRight;
            BottomLeft = bottomLeft;
        }

        public BorderRadius(float radius)
        {
            TopLeft = radius;
            TopRight = radius;
            BottomRight = radius;
            BottomLeft = radius;
        }

        public static implicit operator BorderRadius(float radius) => new BorderRadius(radius);
    }
}
