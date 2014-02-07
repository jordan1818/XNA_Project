using System;
using ECS;

namespace Game.Components
{
    // Provides a spatial form to an entity.
    public class SpatialFormComponent : IComponent
    {
        // Gets or sets the filename.
        public string SpatialFormFile { get; set; }

        public SpatialFormComponent(string file)
        {
            SpatialFormFile = file;
        }
    }
}
