using System.Collections.Generic;

namespace XRAccelerator.Services
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<int, object> serviceMap;

        static ServiceLocator()
        {
            serviceMap = new Dictionary<int, object>();
        }

        public static void RegisterService<T>(T service) where T : class
        {
            serviceMap[typeof(T).GetHashCode()] = service;
        }

        public static void DeregisterService<T>() where T : class
        {
            serviceMap.Remove(typeof(T).GetHashCode());
        }

        public static T GetService<T>() where T : class
        {
            object service;
            serviceMap.TryGetValue(typeof(T).GetHashCode(), out service);
            return (T) service;
        }

        public static void Clear()
        {
            serviceMap.Clear();
        }
    }
}