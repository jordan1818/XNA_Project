using System;
using Microsoft.Xna.Framework;
using ECS;

namespace Game.Components
{
    // Basic component for any entity with a position in 3D space.
    public class TransformComponent : IComponent
    {
        // The position of the entity.
        public Vector3 Position { get; set; }

        // The scale of the entity.
        public Vector3 Scale { get; set; }

        // The rotation of the entity.
        public Quaternion Rotation { get; set; }

        // The full transformation matrix. Lazily calculated.
        public Matrix TransformMatrix
        {
            get
            {
                return Matrix.CreateScale(Scale) * Matrix.CreateFromQuaternion(Rotation) * Matrix.CreateTranslation(Position);
            }
        }

        public TransformComponent()
        {
            Position = Vector3.Zero;
            Scale    = Vector3.One;
            Rotation = Quaternion.Identity;
        }
    }
}
