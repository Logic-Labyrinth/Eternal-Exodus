using System.Collections;
using UnityEngine;

namespace TEE.Environment {
    public class BreakableObject : MonoBehaviour {
        [SerializeField] GameObject objectWhole;
        [SerializeField] GameObject objectBroken;
        [SerializeField] float      destroyTime = 10f;

        void Start() {
            objectWhole.SetActive(true);
            objectBroken.SetActive(false);
        }

        public void Break() {
            objectWhole.SetActive(false);
            objectBroken.SetActive(true);
            StartCoroutine(DestroyObject());
        }

        IEnumerator DestroyObject() {
            yield return new WaitForSeconds(destroyTime);
            GetComponentInChildren<Collider>().enabled = false;

            yield return new WaitForSeconds(5f);
            Destroy(gameObject);
        }
    }
}