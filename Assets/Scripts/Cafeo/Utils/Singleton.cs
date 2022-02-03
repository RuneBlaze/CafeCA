using UnityEngine;

namespace Cafeo.Utils
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        public static T Instance { get; private set; }

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