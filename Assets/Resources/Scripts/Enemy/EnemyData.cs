namespace TEE.Enemy {
    public interface EnemyData {
        protected EnemyType Type   { get; }
        protected int       Health { get; }
    }
}