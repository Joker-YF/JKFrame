using UnityEngine;

namespace JKFrame
{
    public abstract class SingletonMono<T> : MonoBehaviour where T : SingletonMono<T>
    {
        public static T Instance;
        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
            }
        }
    }
}

