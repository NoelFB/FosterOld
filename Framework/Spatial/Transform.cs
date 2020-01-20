using System;

namespace Foster.Framework
{
    /// <summary>
    /// An interface that implements a 3D Transform
    /// </summary>
    public interface ITransform
    {
        Vector3 Position { get; set; }
        Vector3 Scale { get; set; }
        Quaternion Rotation { get; set; }
    }

    /// <summary>
    /// A 3D Transform
    /// </summary>
    public class Transform : ITransform
    {
        public event Action? OnChanged;

        private Transform? parent;
        private Vector3 position;
        private Vector3 scale = Vector3.One;
        private Quaternion rotation;
        private Matrix matrix;
        private Matrix inverse;
        private Vector3 forward;
        private Vector3 left;
        private Vector3 right;
        private Vector3 backward;
        private Vector3 up;
        private Vector3 down;
        private bool dirty = true;

        public Transform? Parent
        {
            get => parent;
            set
            {
                if (parent != value)
                {
                    if (parent != null)
                        parent.OnChanged -= MakeDirty;

                    parent = value;

                    if (parent != null)
                        parent.OnChanged += MakeDirty;

                    MakeDirty();
                }
            }
        }

        public float X
        {
            get => Position.X;
            set => Position = new Vector3(value, Position.Y, Position.Z);
        }

        public float Y
        {
            get => Position.Y;
            set => Position = new Vector3(Position.X, value, Position.Z);
        }

        public float Z
        {
            get => Position.Z;
            set => Position = new Vector3(Position.X, Position.Y, value);
        }

        public float ScaleX
        {
            get => Scale.X;
            set => Scale = new Vector3(value, Scale.Y, Scale.Z);
        }

        public float ScaleY
        {
            get => Scale.Y;
            set => Scale = new Vector3(Scale.X, value, Scale.Z);
        }

        public float ScaleZ
        {
            get => Scale.Z;
            set => Scale = new Vector3(Scale.X, Scale.Y, value);
        }

        public Vector3 Position
        {
            get => position;
            set
            {
                if (position != value)
                {
                    position = value;
                    MakeDirty();
                }
            }
        }

        public Vector3 Scale
        {
            get => scale;
            set
            {
                if (scale != value)
                {
                    scale = value;
                    MakeDirty();
                }
            }
        }

        public Quaternion Rotation
        {
            get => rotation;
            set
            {
                if (rotation != value)
                {
                    rotation = value;
                    MakeDirty();
                }
            }
        }

        public Matrix Matrix
        {
            get
            {
                if (dirty)
                    Update();

                return matrix;
            }
        }

        public Matrix Inverse
        {
            get
            {
                if (dirty)
                    Update();

                return inverse;
            }
        }

        public Vector3 Forward
        {
            get
            {
                if (dirty)
                    Update();
                return forward;
            }
        }

        public Vector3 Backward
        {
            get
            {
                if (dirty)
                    Update();
                return backward;
            }
        }

        public Vector3 Left
        {
            get
            {
                if (dirty)
                    Update();
                return left;
            }
        }

        public Vector3 Right
        {
            get
            {
                if (dirty)
                    Update();
                return right;
            }
        }

        public Vector3 Up
        {
            get
            {
                if (dirty)
                    Update();
                return up;
            }
        }

        public Vector3 Down
        {
            get
            {
                if (dirty)
                    Update();
                return down;
            }
        }

        public Vector3 GlobalPosition
        {
            get
            {
                if (parent != null)
                    return Vector3.Transform(position, parent.Matrix);
                return position;
            }
        }

        private void Update()
        {
            matrix = Matrix.CreateScale(scale) *
                     Matrix.CreateFromQuaternion(rotation) *
                     Matrix.CreateTranslation(position);

            forward = Vector3.Transform(Vector3.Forward, rotation);
            backward = -forward;
            left = Vector3.Transform(Vector3.Left, rotation);
            right = -left;
            up = Vector3.Transform(Vector3.Up, rotation);
            down = -up;

            if (parent != null)
                matrix *= parent.Matrix;

            Matrix.Invert(matrix, out inverse);

            dirty = false;
        }

        private void MakeDirty()
        {
            dirty = true;
            OnChanged?.Invoke();
        }
    }
}
