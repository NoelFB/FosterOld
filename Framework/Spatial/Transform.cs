using System;
using System.Numerics;

namespace Foster.Framework
{
    /// <summary>
    /// An interface that implements a 3D Transform
    /// </summary>
    public interface ITransform
    {
        /// <summary>
        /// Gets or Sets the World Position of the Transform
        /// </summary>
        Vector3 Position { get; set; }

        /// <summary>
        /// Gets or Sets the World Scale of the Transform
        /// </summary>
        Vector3 Scale { get; set; }

        /// <summary>
        /// Gets or Sets the World Rotation of the Transform
        /// </summary>
        Quaternion Rotation { get; set; }
    }

    /// <summary>
    /// A 3D Transform
    /// </summary>
    public class Transform : ITransform
    {
        private Transform? parent = null;
        private bool dirty = true;

        private Vector3 position = Vector3.Zero;
        private Vector3 localPosition = Vector3.Zero;
        private Vector3 scale = Vector3.Zero;
        private Vector3 localScale = Vector3.One;
        private Quaternion rotation = Quaternion.Identity;
        private Quaternion localRotation = Quaternion.Identity;

        private Matrix4x4 localMatrix = Matrix4x4.Identity;
        private Matrix4x4 worldMatrix = Matrix4x4.Identity;
        private Matrix4x4 worldToLocalMatrix = Matrix4x4.Identity;

        /// <summary>
        /// An action called whenever the Transform is modified
        /// </summary>
        public event Action? OnChanged;

        /// <summary>
        /// Gets or Sets the Transform's Parent
        /// Modifying this does not change the World position of the Transform
        /// </summary>
        public Transform? Parent
        {
            get => parent;
            set
            {
                if (parent != value)
                {
                    // Circular Hierarchy isn't allowed
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
                    Position = position;
                    Scale = scale;
                    Rotation = rotation;

                    // Add our OnChanged listener to the new parent
                    if (parent != null)
                        parent.OnChanged += MakeDirty;

                    // we have changed
                    OnChanged?.Invoke();
                }
            }
        }

        /// <summary>
        /// Gets or Sets the World Position of the Transform
        /// </summary>
        public Vector3 Position
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
                    LocalPosition = Vector3.Transform(value, WorldToLocalMatrix);
            }
        }

        /// <summary>
        /// Gets or Sets the X Component of the World Position of the Transform
        /// </summary>
        public float X
        {
            get => Position.X;
            set => Position = new Vector3(value, Position.Y, Position.Z);
        }

        /// <summary>
        /// Gets or Sets the Y Component of the World Position of the Transform
        /// </summary>
        public float Y
        {
            get => Position.Y;
            set => Position = new Vector3(Position.X, value, Position.Z);
        }

        /// <summary>
        /// Gets or Sets the Z Component of the World Position of the Transform
        /// </summary>
        public float Z
        {
            get => Position.Z;
            set => Position = new Vector3(Position.X, Position.Y, value);
        }

        /// <summary>
        /// Gets or Sets the Local Position of the Transform
        /// </summary>
        public Vector3 LocalPosition
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
        public Vector3 Scale
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

                    if (parent.Scale.Z == 0)
                        value.Z = 0;
                    else
                        value.Z /= parent.Scale.Z;

                    LocalScale = value;
                }
            }
        }

        /// <summary>
        /// Gets or Sets the Local Scale of the Transform
        /// </summary>
        public Vector3 LocalScale
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
        public Quaternion Rotation
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
                    LocalRotation = value / parent.Rotation;
            }
        }

        /// <summary>
        /// Gets or Sets the Local Rotation of the Transform
        /// </summary>
        public Quaternion LocalRotation
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
        public Matrix4x4 LocalMatrix
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
        public Matrix4x4 WorldMatrix
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
        public Matrix4x4 WorldToLocalMatrix
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

            localMatrix = 
                Matrix4x4.CreateTranslation(localPosition) *
                Matrix4x4.CreateFromQuaternion(localRotation) *
                Matrix4x4.CreateScale(localScale);

            if (parent == null)
            {
                worldMatrix = localMatrix;
                worldToLocalMatrix = Matrix4x4.Identity;
                position = localPosition;
                scale = localScale;
                rotation = localRotation;
            }
            else
            {
                worldMatrix = localMatrix * parent.WorldMatrix;
                Matrix4x4.Invert(parent.WorldMatrix, out worldToLocalMatrix);
                position = Vector3.Transform(localPosition, parent.WorldMatrix);
                scale = localScale * parent.Scale;
                rotation = localRotation * parent.Rotation;
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
