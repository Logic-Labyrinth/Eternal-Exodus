using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LexUtils.Events {
    public class SuperEvent : UnityEvent {
        readonly Dictionary<string, UnityEvent> channels = new();

        public UnityEvent Get(string channel = "") {
            channels.TryAdd(channel, new UnityEvent());
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
                if (channels.TryGetValue(channel, out var unityEvent))
                    unityEvent.RemoveAllListeners();
            }

            channels.Clear();
        }
    }
}