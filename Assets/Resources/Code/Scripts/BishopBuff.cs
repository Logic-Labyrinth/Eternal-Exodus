using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BishopBuff : MonoBehaviour
{
    [SerializeField] SphereCollider buffArea;
    List<GameObject> pawns = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        if (!buffArea) buffArea = GetComponent<SphereCollider>();
    }

    void FixedUpdate() {
        // if a pawn gameObject in the list becomes inactive, remove it from the list
        for (int i = pawns.Count - 1; i >= 0; i--) {
            if (pawns[i].activeSelf == false) {
                pawns.RemoveAt(i);
            }
        }
    }

    // give pawn shield from health system
    void OnTriggerStay(Collider other) {
        Debug.Log("Something entered me: " + other.gameObject.name);
        if (other.gameObject.tag == "Enemy" && other.gameObject.GetParent().tag == "Pawn") {
            if (!pawns.Contains(other.gameObject)) {
                other.GetComponentInParent<HealthSystem>().Shield();
                Debug.Log("Shielded");
                pawns.Add(other.gameObject);
            }
        }
    }
}
