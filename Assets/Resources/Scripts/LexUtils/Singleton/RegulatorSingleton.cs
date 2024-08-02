using UnityEngine;

namespace LexUtils.Singleton {
    /// <summary>
    /// Persistent Regulator Singleton, will destroy any other older components of the same type it finds on awake
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RegulatorSingleton<T> : MonoBehaviour where T : Component {
        static T instance;

        float InitializationTime { get; set; }

        public static T Instance {
            get {
                if (instance) return instance;
                instance = FindAnyObjectByType<T>();
                if (instance) return instance;

                var go = new GameObject(typeof(T).Name + " Auto-Generated") {
                    hideFlags = HideFlags.HideAndDontSave
                };
                Debug.LogWarning($"No instance of {typeof(T).Name} found. Auto-creating one at {go.name}.");
                instance = go.AddComponent<T>();
                return instance;
            }
        }

        protected virtual void Awake() => InitializeSingleton();

        void InitializeSingleton() {
            if (!Application.isPlaying) return;
            InitializationTime = Time.time;
            DontDestroyOnLoad(gameObject);

            var oldInstances = FindObjectsByType<T>(FindObjectsSortMode.None);
            foreach (var old in oldInstances) {
                if (old.GetComponent<RegulatorSingleton<T>>().InitializationTime < InitializationTime)
                    Destroy(old.gameObject);
            }

            if (instance == null) instance = this as T;
        }
    }
}