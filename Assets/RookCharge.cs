using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RookCharge : MonoBehaviour
{
    public EnemyAI self;

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag == "Player" && self.isCharging) {
                other.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * 100 + Vector3.up * 40, ForceMode.Impulse);
                self.isCharging = false;
        }
    }
}