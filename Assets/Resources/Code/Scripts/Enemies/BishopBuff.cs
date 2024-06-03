using UnityEngine;
using UnityEngine.VFX;

public class BishopBuff : MonoBehaviour {
    [SerializeField] GameObject buffAura;
    [SerializeField] float range = 10f;
    Collider[] colliders;

    void Start() {
        int pawnCount = SpawnManager.Instance.PawnPoolSize;
        colliders = new Collider[pawnCount];
        buffAura.GetComponent<SphereCollider>().radius = range;
        buffAura.GetComponent<VisualEffect>().SetFloat("Size", range * 2);
    }

    void FixedUpdate() {
        int pawnCount = Physics.OverlapSphereNonAlloc(transform.position, range, colliders, LayerMask.NameToLayer("Enemy"));
        for (int i = 0; i < pawnCount; i++) {
            if (colliders[i].gameObject.CompareTag("Pawn"))
                colliders[i].GetComponent<HealthSystem>().Shield();
        }
    }
}
