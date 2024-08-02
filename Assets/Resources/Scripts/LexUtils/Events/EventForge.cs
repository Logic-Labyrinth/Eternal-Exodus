namespace LexUtils.Events {
    public static class EventForge {
        public static readonly SuperEvent Signal = new();

        public static void UnregisterAllEvents() {
            Signal.RemoveAllChannels();
        }
    }
}