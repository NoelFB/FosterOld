using System;
using System.Numerics;

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

        private Matrix3x2 localMatrix = Matrix3x2.Identity;
        private Matrix3x2 worldMatrix = Matrix3x2.Identity;
        private Matrix3x2 worldToLocalMatrix = Matrix3x2.Identity;

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

        public Matrix3x2 LocalMatrix
        {
            get
            {
                if (dirty)
                    Update();
                return localMatrix;
            }
        }

        public Matrix3x2 WorldMatrix
        {
            get
            {
                if (dirty)
                    Update();
                return worldMatrix;
            }
        }

        public Matrix3x2 WorldToLocalMatrix
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

            localMatrix = CreateMatrix(localPosition, origin, localScale, localRotation);

            if (parent == null)
            {
                worldMatrix = localMatrix;
                worldToLocalMatrix = Matrix3x2.Identity;
                position = localPosition;
                scale = localScale;
                rotation = localRotation;
            }
            else
            {
                worldMatrix = localMatrix * parent.WorldMatrix;
                Matrix3x2.Invert(parent.worldMatrix, out worldToLocalMatrix);
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

        public static Matrix3x2 CreateMatrix(in Vector2 position, in Vector2 origin, in Vector2 scale, in float rotation)
        {
            Matrix3x2 matrix;

            if (origin != Vector2.Zero)
                matrix = Matrix3x2.CreateTranslation(-origin.X, -origin.Y);
            else
                matrix = Matrix3x2.Identity;

            if (scale != Vector2.One)
                matrix *= Matrix3x2.CreateScale(scale.X, scale.Y);

            if (rotation != 0)
                matrix *= Matrix3x2.CreateRotation(rotation);

            if (position != Vector2.Zero)
                matrix *= Matrix3x2.CreateTranslation(position.X, position.Y);

            return matrix;
        }
    }
}
