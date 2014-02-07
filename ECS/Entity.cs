using System;
using System.Collections.Generic;

namespace ECS
{
    public sealed class Entity
    {
        // A unique ID representing the Entity. Will not be reused if
        // the entity is deleted.
        public long UniqueID { get; internal set; }

        // The entity's components mapped to their type.
        private Dictionary<Type, IComponent> components;

        internal Entity(long id)
        {
            UniqueID = id;
        }

        // Attempt to retrieve a component of type T. 
        public T GetComponent<T>() where T:IComponent
        {
            IComponent c;
            if (components.TryGetValue(typeof(T), out c))
            {
                return (T)c;
            }
            throw new ArgumentException(String.Format("Entity {0} does not contain component {1}", UniqueID, typeof(T)));
        }

        // Add a component of type T if it does not already exist.
        public void AddComponent<T>(T component) where T:IComponent
        {
            if (!components.ContainsKey(typeof(T)))
            {
                components[typeof(T)] = component;
            }
        }

        // Remove component of type T.
        public void RemoveComponent<T>()
        {
            components.Remove(typeof(T));
        }

        // Check if Entity has component of type T.
        public bool HasComponent<T>()
        {
            return HasComponent(typeof(T));
        }

        public bool HasComponent(Type t)
        {
            return components.ContainsKey(t);
        }
    }
}
