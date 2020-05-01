using System;
using System.Numerics;

namespace Foster.Framework
{
    /// <summary>
    /// An interface that implements a 2D Transform
    /// </summary>
    public interface ITransform2D
    {
        /// <summary>
        /// Gets or Sets the World Position of the Transform
        /// </summary>
        Vector2 Position { get; set; }

        /// <summary>
        /// Gets or Sets the World Scale of the Transform
        /// </summary>
        Vector2 Scale { get; set; }

        /// <summary>
        /// Gets or Sets the Origin of the Transform
        /// </summary>
        Vector2 Origin { get; set; }

        /// <summary>
        /// Gets or Sets the World Rotation of the Transform
        /// </summary>
        float Rotation { get; set; }
    }

    /// <summary>
    /// A 2D Transform
    /// </summary>
    public class Transform2D : ITransform2D
    {
        private Transform2D? parent = null;
        private bool dirty = true;

        private Vector2 position = Vector2.Zero;
        private Vector2 localPosition = Vector2.Zero;
        private Vector2 scale = Vector2.One;
        private Vector2 localScale = Vector2.One;
        private Vector2 origin = Vector2.Zero;
        private float rotation = 0f;
        private float localRotation = 0f;

        private Matrix3x2 localMatrix = Matrix3x2.Identity;
        private Matrix3x2 worldMatrix = Matrix3x2.Identity;
        private Matrix3x2 worldToLocalMatrix = Matrix3x2.Identity;

        /// <summary>
        /// An action called whenever the Transform is modified
        /// </summary>
        public event Action? OnChanged;

        /// <summary>
        /// Gets or Sets the Transform's Parent
        /// Modifying this does not change the World position of the Transform
        /// </summary>
        public Transform2D? Parent
        {
            get => parent;
            set => SetParent(value, true);
        }

        /// <summary>
        /// Gets or Sets the Origin of the Transform
        /// This is useful for setting Rotation Origins for things like Sprites
        /// </summary>
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

        /// <summary>
        /// Gets or Sets the World Position of the Transform
        /// </summary>
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
                    LocalPosition = Vector2.Transform(value, WorldToLocalMatrix);
            }
        }

        /// <summary>
        /// Gets or Sets the X Component of the World Position of the Transform
        /// </summary>
        public float X
        {
            get => Position.X;
            set => Position = new Vector2(value, Position.Y);
        }

        /// <summary>
        /// Gets or Sets the Y Component of the World Position of the Transform
        /// </summary>
        public float Y
        {
            get => Position.Y;
            set => Position = new Vector2(Position.X, value);
        }

        /// <summary>
        /// Gets or Sets the Local Position of the Transform
        /// </summary>
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

        /// <summary>
        /// Gets or Sets the World Scale of the Transform
        /// </summary>
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

        /// <summary>
        /// Gets or Sets the Local Scale of the Transform
        /// </summary>
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

        /// <summary>
        /// Gets or Sets the World Rotation of the Transform
        /// </summary>
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

        /// <summary>
        /// Gets or Sets the Local Rotation of the Transform
        /// </summary>
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

        /// <summary>
        /// Gets the Local Matrix of the Transform
        /// </summary>
        public Matrix3x2 LocalMatrix
        {
            get
            {
                if (dirty)
                    Update();
                return localMatrix;
            }
        }

        /// <summary>
        /// Gets the World Matrix of the Transform
        /// </summary>
        public Matrix3x2 WorldMatrix
        {
            get
            {
                if (dirty)
                    Update();
                return worldMatrix;
            }
        }

        /// <summary>
        /// Gets the World-to-Local Matrix of the Transform
        /// </summary>
        public Matrix3x2 WorldToLocalMatrix
        {
            get
            {
                if (dirty)
                    Update();
                return worldToLocalMatrix;
            }
        }

        /// <summary>
        /// Sets the Parent of this Transform
        /// </summary>
        /// <param name="value">The new Parent</param>
        /// <param name="retainWorldPosition">Whether this Transform should retain its world position when it is transfered to the new parent</param>
        public void SetParent(Transform2D? value, bool retainWorldPosition)
        {
            if (parent != value)
            {
                // Circular Hierarchy isn't allowed
                // TODO: this only checks 1 parent, instead of the whole tree
                if (value != null && value.Parent == this)
                    throw new Exception("Circular Transform Heritage is not allowed");

                // Remove our OnChanged listener from the existing parent
                if (parent != null)
                    parent.OnChanged -= MakeDirty;

                // store state
                var position = Position;
                var scale = Scale;
                var rotation = Rotation;

                // update parent
                parent = value;
                dirty = true;

                // retain state
                if (retainWorldPosition)
                {
                    Position = position;
                    Scale = scale;
                    Rotation = rotation;
                }

                // Add our OnChanged listener to the new parent
                if (parent != null)
                    parent.OnChanged += MakeDirty;

                // we have changed
                OnChanged?.Invoke();
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

        /// <summary>
        /// Creates a Matrix3x2 given the Transform Values
        /// </summary>
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
