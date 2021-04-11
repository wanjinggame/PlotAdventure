using UnityEngine;

namespace Plot.Utility
{
    public class Singleton<T> where T : class, new ()
    {
        private static T _instance;
        public static T Instance()
        {
            if (_instance == null)
            {
                if (_instance == null)
                {
                    _instance = new T();
                }
            }
            return _instance;
        }
    }

    public class TSingleton<T> where T : TSingleton<T>
    {
        private static T instance = null;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = System.Activator.CreateInstance<T>();
                    instance.OnCreateInstance();
                }

                return instance;
            }
        }

        public static bool HasInstance()
        {
            return instance != null;
        }

        public static void TryResetInstance()
        {
            if (instance != null)
            {
                instance.OnResetInstance();
                instance = default(T);
            }
        }

        protected virtual void OnCreateInstance()
        {
        }

        protected virtual void OnResetInstance()
        {

        }
    }

    public class TSingletonMono<T> : MonoBehaviour where T : TSingletonMono<T>
    {
        private static T instance = null;

        public static T Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }

                instance = FindObjectOfType(typeof(T)) as T;

                if (instance == null)
                {
                    string instanceGameObjectName = typeof(T).ToString();

                    instance = new GameObject(instanceGameObjectName).AddComponent<T>();
                    DontDestroyOnLoad(instance);
                    instance.OnCreateInstance();
                }

                return instance;
            }
        }

        public static bool HasInstance()
        {
            return instance != null;
        }

        public static void TryResetInstance()
        {
            if (instance != null)
            {
                DestroyImmediate(instance.gameObject);
                instance = default(T);
            }
        }

        protected virtual void OnCreateInstance()
        {
        }

        protected virtual void OnResetInstance()
        {

        }

        private void OnDestroy()
        {
            if (instance != null)
            {
                instance.OnResetInstance();
            }
            instance = default(T);
        }
    }
}