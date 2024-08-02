using UnityEngine;

namespace LexUtils.Singleton {
    /// <summary>
    /// Persistent Singleton, will destroy any new components of the same type it finds on awake
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PersistentSingleton<T> : MonoBehaviour where T : Component {
        [HideInInspector] public bool autoUnparentOnAwake = true;

        static T instance;

        public static T Instance {
            get {
                if (instance) return instance;
                instance = FindAnyObjectByType<T>();
                if (instance) return instance;

                var go = new GameObject(typeof(T).Name + " Auto-Generated");
                Debug.LogWarning($"No instance of {typeof(T).Name} found. Auto-creating one at {go.name}.");
                instance = go.AddComponent<T>();
                return instance;
            }
        }

        protected virtual void Awake() => InitializeSingleton();

        void InitializeSingleton() {
            if (!Application.isPlaying) return;
            if (autoUnparentOnAwake) transform.SetParent(null);

            if (instance == null) {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
            } else if (instance != this) Destroy(gameObject);
        }
    }
}