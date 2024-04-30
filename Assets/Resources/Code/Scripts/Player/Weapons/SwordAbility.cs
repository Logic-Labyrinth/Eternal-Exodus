using System.Collections;
using UnityEngine;

public class SwordAbility : MonoBehaviour {
    [SerializeField] Weapon spearData;

    // public void Uppercut(Collider enemy) {
    //     StartCoroutine(DelayedUppercut(enemy));
    // }

    // IEnumerator DelayedUppercut(Collider enemy) {
    //     yield return new WaitForSeconds(0.5f);
    //     enemy.GetComponent<Rigidbody>().velocity = Vector3.zero;
    //     enemy.GetComponent<Rigidbody>().AddForce(Vector3.up * 50, ForceMode.Impulse);
    // }
}
