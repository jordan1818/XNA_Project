using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECS
{
    /// <summary>
    /// Blackboard provides a way of sharing objects between systems.
    /// </summary>
    public static class BlackBoard
    {
        /// <summary>
        /// Mapping of object names to objects.
        /// </summary>
        private static Dictionary<string, object> objects = new Dictionary<string, object>();

        /// <summary>
        /// Retrieve an entry in the blackboard. It will be converted to type T before being returned.
        /// </summary>
        /// <typeparam name="T">The object's real type</typeparam>
        /// <param name="key">the object's ID.</param>
        /// <returns>The object type-casted to type T.</returns>
        public static T GetEntry<T>(string key)
        {
            object val;
            if (!objects.TryGetValue(key, out val))
            {
                throw new KeyNotFoundException("Blackboard could not find " + key);
            }
            return (T)val;
        }

        /// <summary>
        /// Set an entry in the blackboard.
        /// </summary>
        /// <param name="key">The object's ID.</param>
        /// <param name="val">The object.</param>
        public static void SetEntry(string key, object val)
        {
            objects[key] = val;
        }

        /// <summary>
        /// Remove an entry in the blackboard.
        /// </summary>
        /// <param name="key">The ID of the object.</param>
        public static void RemoveEntry(string key)
        {
            objects.Remove(key);
        }
    }
}
