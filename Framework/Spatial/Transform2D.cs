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

        private Transform2D? parent = null;

        private Vector2 position = Vector2.Zero;
        private Vector2 localPosition = Vector2.Zero;
        private Vector2 scale = Vector2.Zero;
        private Vector2 localScale = Vector2.One;
        private Vector2 origin = Vector2.Zero;
        private float rotation = 0f;
        private float localRotation = 0f;

        private bool dirty = true;

        private Matrix2D localMatrix = Matrix2D.Identity;
        private Matrix2D worldMatrix = Matrix2D.Identity;
        private Matrix2D worldToLocalMatrix = Matrix2D.Identity;

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

        public Vector2 Position
        {
            get
            {
                if (dirty)
                    Update();

                return position;
            }
            set
            {
                if (parent == null)
                    LocalPosition = value;
                else
                    LocalPosition = Vector2.Transform(value, worldToLocalMatrix);
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

        public Vector2 LocalPosition
        {
            get => localPosition;
            set
            {
                if (localPosition != value)
                {
                    localPosition = value;
                    MakeDirty();
                }
            }
        }

        public Vector2 Scale
        {
            get
            {
                if (dirty)
                    Update();

                return scale;
            }
            set
            {
                if (parent == null)
                {
                    LocalScale = value;
                }
                else
                {
                    if (parent.Scale.X == 0)
                        value.X = 0;
                    else
                        value.X /= parent.Scale.X;

                    if (parent.Scale.Y == 0)
                        value.Y = 0;
                    else
                        value.Y /= parent.Scale.Y;

                    LocalScale = value;
                }
            }
        }

        public Vector2 LocalScale
        {
            get => localScale;
            set
            {
                if (localScale != value)
                {
                    localScale = value;
                    MakeDirty();
                }
            }
        }

        public float Rotation
        {
            get
            {
                if (dirty)
                    Update();

                return rotation;
            }
            set
            {
                if (parent == null)
                    LocalRotation = value;
                else
                    LocalRotation = value - parent.Rotation;
            }
        }

        public float LocalRotation
        {
            get => localRotation;
            set
            {
                if (localRotation != value)
                {
                    localRotation = value;
                    MakeDirty();
                }
            }
        }

        public Matrix2D LocalMatrix
        {
            get
            {
                if (dirty)
                    Update();
                return localMatrix;
            }
        }

        public Matrix2D WorldMatrix
        {
            get
            {
                if (dirty)
                    Update();
                return worldMatrix;
            }
        }

        public Matrix2D WorldToLocalMatrix
        {
            get
            {
                if (dirty)
                    Update();
                return worldToLocalMatrix;
            }
        }

        private void Update()
        {
            dirty = false;

            localMatrix = Matrix2D.CreateTransform(localPosition, origin, localScale, localRotation);

            if (parent == null)
            {
                worldMatrix = localMatrix;
                worldToLocalMatrix = Matrix2D.Identity;
                position = localPosition;
                scale = localScale;
                rotation = localRotation;
            }
            else
            {
                worldMatrix = localMatrix * parent.WorldMatrix;
                worldToLocalMatrix = parent.WorldMatrix.Invert();
                position = Vector2.Transform(localPosition, parent.WorldMatrix);
                scale = localScale * parent.Scale;
                rotation = localRotation + parent.Rotation;
            }

        }

        private void MakeDirty()
        {
            if (!dirty)
            {
                dirty = true;
                OnChanged?.Invoke();
            }
        }

    }
}
