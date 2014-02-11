using System;
using ECS;

namespace Game.Components
{
    /// <summary>
    /// Provides a spatial form to an entity.
    /// </summary>
    public class SpatialFormComponent : IComponent
    {
        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        public string SpatialFormFile { get; set; }

        public SpatialFormComponent(string file)
        {
            SpatialFormFile = file;
        }
    }
}
