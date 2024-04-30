using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class PortalVFX : MonoBehaviour {
    VisualEffect portalVFX;

    void Start() {
        portalVFX = GetComponent<VisualEffect>();
        portalVFX.Stop();
        transform.localScale = Vector3.zero;
    }

    public void OpenPortal() {
        portalVFX.Play();
        StartCoroutine(IncreasePortalSize());
    }

    IEnumerator IncreasePortalSize() {
        float time = 0;
        float duration = 1;

        while (time < duration) {
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, time);
            time += Time.deltaTime;
            yield return null;
        }
    }

    public void ClosePortal() {
        portalVFX.Stop();
    }

    void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.CompareTag("Player")) {
            ClosePortal();

            // GameManager.Instance.LoadScene("730181");
            FindObjectOfType<EndScreenController>(true).gameObject.SetActive(true);
        }
    }
}
