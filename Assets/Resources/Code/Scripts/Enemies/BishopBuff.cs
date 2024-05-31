using UnityEngine;

public class BishopBuff : MonoBehaviour {
    [SerializeField] float range = 10f;
    Collider[] colliders;

    void Start() {
        int pawnCount = SpawnManager.Instance.PawnPoolSize;
        colliders = new Collider[pawnCount];
    }

    void FixedUpdate() {
        int pawnCount = Physics.OverlapSphereNonAlloc(transform.position, range, colliders, LayerMask.NameToLayer("Enemy"));
        for (int i = 0; i < pawnCount; i++) {
            if (colliders[i].gameObject.CompareTag("Pawn"))
                colliders[i].GetComponent<HealthSystem>().Shield();
        }
    }
}
