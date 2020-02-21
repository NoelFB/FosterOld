using System;
using System.Numerics;

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
    /// A 2D Transform
    /// </summary>
    public class Transform : ITransform
    {
        public event Action? OnChanged;

        private Transform? parent = null;

        private Vector3 position = Vector3.Zero;
        private Vector3 localPosition = Vector3.Zero;
        private Vector3 scale = Vector3.Zero;
        private Vector3 localScale = Vector3.One;
        private Quaternion rotation = Quaternion.Identity;
        private Quaternion localRotation = Quaternion.Identity;

        private bool dirty = true;

        private Matrix4x4 localMatrix = Matrix4x4.Identity;
        private Matrix4x4 worldMatrix = Matrix4x4.Identity;
        private Matrix4x4 worldToLocalMatrix = Matrix4x4.Identity;

        public Transform? Parent
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
                    LocalPosition = Vector3.Transform(value, worldToLocalMatrix);
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

        public Matrix4x4 LocalMatrix
        {
            get
            {
                if (dirty)
                    Update();
                return localMatrix;
            }
        }

        public Matrix4x4 WorldMatrix
        {
            get
            {
                if (dirty)
                    Update();
                return worldMatrix;
            }
        }

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
