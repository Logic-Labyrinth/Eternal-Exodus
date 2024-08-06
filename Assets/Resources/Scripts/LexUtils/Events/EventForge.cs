using UnityEngine;

namespace LexUtils.Events {
    public static class EventForge {
        public static readonly SuperEvent Signal = new();
        public static readonly SuperEvent<Vector2> Vector2 = new();
        public static readonly SuperEvent<Vector3> Vector3 = new();
        public static readonly SuperEvent<float> Float = new();
        public static readonly SuperEvent<int> Integer = new();

        public static void UnregisterAllEvents() {
            Signal.RemoveAllChannels();
            Vector2.RemoveAllChannels();
            Vector3.RemoveAllChannels();
            Float.RemoveAllChannels();
            Integer.RemoveAllChannels();
        }
    }
}