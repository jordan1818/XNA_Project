using System;
using Microsoft.Xna.Framework;
using ECS;

namespace Game.Components
{
    /// <summary>
    /// Basic component for any entity with a position in 3D space.
    /// </summary>
    public class TransformComponent : IComponent
    {
        /// <summary>
        ///  The position of the entity.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// The scale of the entity.
        /// </summary>
        public Vector3 Scale { get; set; }

        /// <summary>
        ///  The rotation of the entity.
        /// </summary>
        public Quaternion Rotation { get; set; }

        /// <summary>
        /// The full transformation matrix. Lazily calculated.
        /// </summary>
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
