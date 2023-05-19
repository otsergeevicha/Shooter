using UnityEngine;

namespace Plugins.MonoCache.System
{
    public abstract class Singleton<TSingleton> : MonoBehaviour where TSingleton : MonoBehaviour
    {
        public static TSingleton Instance
        {
            get
            {
                lock (SecurityLock)
                    return GetInstance();
            }
        }

        public static TSingleton InstanceNonLock => 
            GetInstance();

        private static readonly object SecurityLock = new();

        private static TSingleton _cachedInstance;

        private static TSingleton GetInstance()
        {
            if (_cachedInstance != null)
                return _cachedInstance;

            var allInstances = FindObjectsOfType<TSingleton>();
            var className = typeof(TSingleton).Name;
            var count = allInstances.Length;
            var instance = count > 0
                ? allInstances[0]
                : new GameObject($"[Singleton] {className}").AddComponent<TSingleton>();
            
            if (count > 1)
            {
                for (var i = 1; i < count; i++) 
                    Destroy(allInstances[i]);
#if DEBUG
                Debug.LogError($"The number of <{className}> on the scene is greater than one!");
#endif
            }

            return _cachedInstance = instance;
        }
    }
}