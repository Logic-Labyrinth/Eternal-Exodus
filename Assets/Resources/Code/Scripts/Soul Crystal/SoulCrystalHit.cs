using UnityEngine;

public class SoulCrystalHit : MonoBehaviour {
    [SerializeField] SoulCollector collector;



    public void Hit() {
        collector.Explode();
        //HitFlash();
    }

}
