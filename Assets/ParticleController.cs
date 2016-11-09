using UnityEngine;

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
			particles[i] = new ParticleSystem.Particle
			{
				startSize = 1f,
				lifetime = 100f,
				startLifetime = 100f,
				startColor = Color.white,
			};
		}

		particleSystem.SetParticles( particles, particles.Length);
	}

	public void UpdateParticles(Body[] bodies)
	{
		for (int i = 0; i < particles.Length; i++)
		{
			particles[i].position = bodies[i].Position;
		}

		particleSystem.Clear();
		particleSystem.SetParticles(particles, particles.Length);
	}

	public void UpdateParticles(float[] positions, bool is3D = false)
	{
		for (int i = 0; i < particles.Length; i++)
		{
			int vectorNumber = i * (is3D ? 3 : 2);
			particles[i].position.Set(positions[vectorNumber], positions[vectorNumber + 1], is3D ? positions[vectorNumber + 2] : 0f);
		}

		particleSystem.Clear();
		particleSystem.SetParticles(particles, particles.Length);
	}
}
