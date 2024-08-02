using UnityEngine;

namespace LexUtils.Singleton {
    public class Singleton<T> : MonoBehaviour where T : Component {
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
            instance = this as T;
        }
    }
}