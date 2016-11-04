using UnityEngine;
using System.Collections;

public class Simulator : MonoBehaviour
{
	//TEMP: For now the program will just initialize with a fixed amount of Bodies
	const int BodyAmount = 500;

	const int SimulationRadius = 20;

	Body[] bodies;

	ParticleController particleController;


	// Use this for initialization
	void Start ()
	{
		CreateRandomBodies(BodyAmount);

		particleController = GetComponent<ParticleController>();
		particleController.Initialize(BodyAmount);
	}

	void CreateRandomBodies(int amount)
	{
		bodies = new Body[amount];
		
		for (int i = 0; i < amount; i++)
		{
			bodies[i] = new Body
			(
				Random.insideUnitCircle * SimulationRadius,
				Vector3.zero,
				10000f
			);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		SimulateCPU();
		particleController.UpdateParticles(bodies);
	}

	void SimulateCPU()
	{
		foreach (Body thisBody in bodies)
		{
			foreach (Body thatBody in bodies)
			{
				if(thisBody != thatBody)
				{
					thisBody.AffectBy(thatBody, Time.deltaTime);
				}
			}

			thisBody.Position += thisBody.Velocity * Time.deltaTime;
		}
	}
}
