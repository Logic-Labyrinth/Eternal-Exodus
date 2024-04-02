using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindGradient : MonoBehaviour {

    public ParticleSystem particles;
    ParticleSystem.Particle[] m_Particles;
    public List<ParticleCollisionEvent> collisionEvents;
    public float fade = 0.02f;
    private AnimationCurve curve = new AnimationCurve();
    // Start is called before the first frame update
    void Start() {

        particles = GetComponent<ParticleSystem>();
        int numParticlesAlive = particles.GetParticles(m_Particles);
        collisionEvents = new List<ParticleCollisionEvent>();
        var trailModule = particles.trails;
        curve.AddKey(1f, 0.0f);
        curve.AddKey(0.0f, 1f);
    }

    void OnParticleCollision(GameObject other)
    {
        int numParticlesAlive = particles.GetParticles(m_Particles);
        print("Colliding");
        var trailModule = particles.trails;
        for(int i = 0; i < numParticlesAlive; i++) {
            trailModule.widthOverTrail = new ParticleSystem.MinMaxCurve(1f, curve);
            print(trailModule.widthOverTrail);
        }
        particles.SetParticles(m_Particles, numParticlesAlive);
    }
}