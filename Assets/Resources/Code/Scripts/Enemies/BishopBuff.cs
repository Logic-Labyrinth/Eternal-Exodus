using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.VFX;

public class BishopBuff : MonoBehaviour {
    [SerializeField] GameObject buffAura;
    [SerializeField] float range = 10f;

    void Start() {
        buffAura.GetComponent<VisualEffect>().SetFloat("Size", range * 2);
    }

    void FixedUpdate() {
        // Collider[] colliders = Physics.OverlapSphere(transform.position, range, LayerMask.NameToLayer("Enemy"));
        Collider[] colliders = Physics.OverlapSphere(transform.position, range);

        colliders.ForEach(c => {
            if (c.gameObject.CompareTag("Pawn")) c.GetComponent<HealthSystem>().Shield();
        });
    }
}
