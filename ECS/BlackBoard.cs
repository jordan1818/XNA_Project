using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECS
{
    // Blackboard provides a way of sharing objects between systems.
    public static class BlackBoard
    {
        private static Dictionary<string, object> objects = new Dictionary<string, object>();

        // Retrieve an entry in the blackboard. It will be converted to type T before being returned.
        public static T GetEntry<T>(string key)
        {
            object val;
            if (!objects.TryGetValue(key, out val))
            {
                throw new KeyNotFoundException("Blackboard could not find " + key);
            }
            return (T)val;
        }

        // Set an entry in the blackboard.
        public static void SetEntry(string key, object val)
        {
            objects[key] = val;
        }

        // Remove an entry in the blackboard.
        public static void RemoveEntry(string key)
        {
            objects.Remove(key);
        }
    }
}
