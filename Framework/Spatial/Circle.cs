namespace Foster.Framework
{
    public struct Circle : IProjectable2D
    {

        public Vector2 Position;
        public float Radius;

        public Circle(Vector2 position, float radius)
        {
            Position = position;
            Radius = radius;
        }

        public Circle(float x, float y, float radius)
        {
            Position = new Vector2(x, y);
            Radius = radius;
        }

        public bool Overlaps(Circle circle, out Vector2 pushout)
        {
            pushout = Vector2.Zero;

            if ((Position - circle.Position).LengthSquared < (Radius + circle.Radius) * (Radius + circle.Radius))
            {
                var distance = (Position - circle.Position).Length;
                var normal = (circle.Position - Position).Normalized;

                pushout = normal * (Radius + circle.Radius - distance);

                return true;
            }

            return false;
        }

        public bool Overlaps(IConvexShape2D shape, out Vector2 pushout)
        {
            return shape.Overlaps(this, out pushout);
        }

        public void Project(Vector2 axis, out float min, out float max)
        {
            min = Vector2.Dot(Position - axis * Radius, axis);
            max = Vector2.Dot(Position + axis * Radius, axis);
        }
    }
}
