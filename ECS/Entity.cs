using System;
using System.Collections.Generic;

namespace ECS
{
    /// <summary>
    /// An Entity is essentially an ID to group components together.
    /// </summary>
    public sealed class Entity
    {
        /// <summary>
        /// A unique ID representing the Entity. Will not be reused if the entity is deleted.
        /// </summary>
        public long UniqueID { get; internal set; }

        /// <summary>
        /// A tag to give a more descriptive name to the entity. (eg. Player)
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// The entity's components mapped to their type.
        /// </summary>
        private Dictionary<Type, IComponent> components;

        internal Entity(long id)
        {
            UniqueID = id;
            components = new Dictionary<Type, IComponent>();
        }

        /// <summary>
        /// Attempt to retrieve a component of type T.
        /// </summary>
        /// <typeparam name="T">The components type.</typeparam>
        /// <returns>A reference to the component.</returns>
        public T GetComponent<T>() where T:IComponent
        {
            IComponent c;
            if (components.TryGetValue(typeof(T), out c))
            {
                return (T)c;
            }
            throw new ArgumentException(String.Format("Entity {0} does not contain component {1}", UniqueID, typeof(T)));
        }

        /// <summary>
        /// Add a component of type T if it does not already exist.
        /// </summary>
        /// <param name="component">The component to be added.</param>
        public void AddComponent(IComponent component)
        {
            var T = component.GetType();
            if (!components.ContainsKey(T))
            {
                components[T] = component;
            }
        }

        /// <summary>
        /// Remove a component of type T.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        public void RemoveComponent<T>()
        {
            components.Remove(typeof(T));
        }

        /// <summary>
        /// Check if the Entity has a component of type T.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <returns>True if has component T.</returns>
        public bool HasComponent<T>()
        {
            return HasComponent(typeof(T));
        }

        /// <summary>
        /// Check if the Entity has a component of type t.
        /// </summary>
        /// <param name="t">Type object representing the component type.</param>
        /// <returns>True if has component t.</returns>
        public bool HasComponent(Type t)
        {
            return components.ContainsKey(t);
        }
    }
}
