using UnityEngine;

namespace Cafeo.Utils
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null) _instance = FindObjectOfType<T>();
                return _instance;
            }
            private set => _instance = value;
        }

        protected void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this as T;
                // DontDestroyOnLoad(this);
                Setup();
            }
        }

        protected virtual void Setup()
        {
        }
    }
}