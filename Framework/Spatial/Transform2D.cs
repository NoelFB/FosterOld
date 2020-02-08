using System;

namespace Foster.Framework
{
    /// <summary>
    /// An interface that implements a 2D Transform
    /// </summary>
    public interface ITransform2D
    {
        Vector2 Position { get; set; }
        Vector2 Scale { get; set; }
        Vector2 Origin { get; set; }
        float Rotation { get; set; }
    }

    /// <summary>
    /// A 2D Transform
    /// </summary>
    public class Transform2D : ITransform2D
    {
        public event Action? OnChanged;

        private Transform2D? parent;
        private Vector2 position;
        private Vector2 origin;
        private Vector2 scale = Vector2.One;
        private float rotation;
        private Matrix2D localMatrix;
        private Matrix2D matrix;
        private Matrix2D inverse;
        private bool dirtyMatrix = true;
        private bool dirtyInverseMatrix = true;

        public Transform2D? Parent
        {
            get => parent;
            set
            {
                if (parent != value)
                {
                    if (parent != null)
                        parent.OnChanged -= MakeDirty;

                    if (value != null && value.Parent == this)
                        throw new Exception("Circular Transform Heritage");

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
            set => Position = new Vector2(value, Position.Y);
        }

        public float Y
        {
            get => Position.Y;
            set => Position = new Vector2(Position.X, value);
        }

        public float ScaleX
        {
            get => Scale.X;
            set => Scale = new Vector2(value, Scale.Y);
        }

        public float ScaleY
        {
            get => Scale.Y;
            set => Scale = new Vector2(Scale.X, value);
        }

        public float OriginX
        {
            get => Origin.X;
            set => Origin = new Vector2(value, Origin.Y);
        }

        public float OriginY
        {
            get => Origin.Y;
            set => Origin = new Vector2(Origin.X, value);
        }

        public Vector2 Position
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

        public Vector2 Origin
        {
            get => origin;
            set
            {
                if (origin != value)
                {
                    origin = value;
                    MakeDirty();
                }
            }
        }

        public Vector2 Scale
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

        public float Rotation
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

        public Matrix2D LocalMatrix
        {
            get
            {
                if (dirtyMatrix)
                    Update();

                return localMatrix;
            }
        }

        public Matrix2D Matrix
        {
            get
            {
                if (dirtyMatrix)
                    Update();

                return matrix;
            }
        }

        public Matrix2D Inverse
        {
            get
            {
                if (dirtyMatrix)
                    Update();

                if (dirtyInverseMatrix)
                {
                    inverse = matrix.Invert();
                    dirtyInverseMatrix = false;
                }

                return inverse;
            }
        }

        public Vector2 GlobalPosition
        {
            get
            {
                if (parent != null)
                    return Vector2.Transform(position, parent.Matrix);
                return position;
            }
            set
            {
                if (parent != null)
                    Position = Vector2.Transform(value, parent.Matrix.Invert());
                else
                    Position = value;
            }
        }

        private void Update()
        {
            localMatrix = Matrix2D.CreateTransform(this);

            if (parent != null)
                matrix = localMatrix * parent.Matrix;
            else
                matrix = localMatrix;

            dirtyInverseMatrix = true;
            dirtyMatrix = false;
        }

        private void MakeDirty()
        {
            dirtyMatrix = true;
            OnChanged?.Invoke();
        }
    }
}
