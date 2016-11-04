using UnityEngine;
using System.Collections;

public class ParticleController : MonoBehaviour
{
	ParticleSystem particleSystem;
	ParticleSystem.Particle[] particles;

	// Use this for initialization
	void Start ()
	{
		particleSystem = GetComponent<ParticleSystem>();
	}

	public void Initialize(int particleCount)
	{
		particles = new ParticleSystem.Particle[particleCount];
		for (int i = 0; i < particles.Length; i++)
		{
			particles[i] = new ParticleSystem.Particle();
		}
	}

	public void UpdateParticles(Body[] bodies)
	{
		for (int i = 0; i < particles.Length; i++)
		{
			particles[i].position = bodies[i].Position;
		}
	}
}
