using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LexUtils.Events {
    public class SuperEvent<T0, T1, T2> : UnityEvent<T0, T1, T2> {
        readonly Dictionary<string, UnityEvent<T0, T1, T2>> channels = new();

        public UnityEvent<T0, T1, T2> Get(string channel = "") {
            channels.TryAdd(channel, new UnityEvent<T0, T1, T2>());
            return channels[channel];
        }

        public void RemoveAllListeners(string channel = "") {
            Debug.Log("Removing all listeners from channel: " + channel);
            if (!channels.TryGetValue(channel, out var unityEvent)) return;
            unityEvent.RemoveAllListeners();
            channels.Remove(channel);
        }

        public void RemoveAllChannels() {
            foreach (var channel in channels.Keys) {
                Debug.Log("Removing all listeners from channel: " + channel);
                if (!channels.TryGetValue(channel, out var unityEvent)) continue;
                unityEvent.RemoveAllListeners();
            }

            channels.Clear();
        }
    }
}