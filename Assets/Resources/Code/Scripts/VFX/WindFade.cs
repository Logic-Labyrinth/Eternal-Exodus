using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindFade : MonoBehaviour {

    public ParticleSystem particles;
    public List<ParticleCollisionEvent> collisionEvents;
    public float fade = 0.02f;
    public float rValue = 1f;
    public float gValue = 1f;
    public float bValue = 1f;
    public float aValue = 1f;

    // Start is called before the first frame update
    void Start() {

        particles = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

  /*  void OnParticleCollision(GameObject other)
    {
        print("Colliding");
        var main = particles.main;
        main.startColor = new Color(rValue, gValue, bValue, aValue - fade);

    }*/
}