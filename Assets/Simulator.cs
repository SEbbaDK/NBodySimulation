using System;
using System.IO;
using System.Linq;
using UnityEngine;
using Cloo;
using Random = UnityEngine.Random;
using Debug = UnityEngine.Debug;

public class Simulator : MonoBehaviour
{
	//TEMP: For now the program will just initialize with a fixed amount of Bodies
	const int BodyAmount = 2;

	const float SimulationRadius = 20f;

	Body[] bodies;
	float[] positions, velocities, masses;

	ComputeCommandQueue queue;
	ComputeKernel kernel;

	ParticleController particleController;

	bool simulate = false;
	bool useGPU = false;

	// Use this for initialization
	void Start ()
	{
		particleController = GetComponent<ParticleController>();
		particleController.Initialize(BodyAmount);

		//TEMP
		SetUpGPU();
	}

	public void SetUpCPU()
	{
		CreateRandomBodiesCPU(BodyAmount);

		simulate = true;
	}

	public void SetUpGPU()
	{
		SetUpCloo();
		CreateRandomBodiesGPU(BodyAmount);

		simulate = true;
		useGPU = true;
	}

	void CreateRandomBodiesCPU(int amount)
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

	void CreateRandomBodiesGPU(int amount, bool is3D = false)
	{
		int dimensions = is3D ? 3 : 2;
		int arrayLength = amount * dimensions;
		positions = new float[arrayLength];
		velocities = new float[arrayLength];
		masses = new float[amount];

		for(int i = 0; i < amount; i++)
		{
			if (is3D)
			{
				Vector3 randomPosition = Random.insideUnitSphere * SimulationRadius;
				positions[(i * dimensions)] = randomPosition.x;
				positions[(i * dimensions) + 1] = randomPosition.y;
				positions[(i * dimensions) + 2] = randomPosition.z;
			}
			else
			{
				Vector2 randomPosition = Random.insideUnitCircle;
				positions[( i * dimensions )] = randomPosition.x;
				positions[( i * dimensions ) + 1] = randomPosition.y;
			}

			masses[i] = 10000f;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!simulate)
			return;

		if (useGPU)
		{
			SimulateGPU();
			particleController.UpdateParticles(positions);
		}
		else
		{
			SimulateCPU();
			particleController.UpdateParticles(bodies);
		}
	}

	void SimulateCPU()
	{
		foreach (Body thisBody in bodies)
		{
			foreach (Body thatBody in bodies)
			{
				if (thisBody != thatBody)
				{
					thisBody.AffectBy(thatBody, Time.deltaTime);
				}
			}

			thisBody.Position += thisBody.Velocity * Time.deltaTime;
		}
	}

	void SimulateGPU()
	{
		queue.Flush();

		ComputeBuffer<float> positionBuffer = new ComputeBuffer<float>(kernel.Context, ComputeMemoryFlags.UseHostPointer | ComputeMemoryFlags.ReadWrite, positions);
		ComputeBuffer<float> velocityBuffer = new ComputeBuffer<float>(kernel.Context, ComputeMemoryFlags.UseHostPointer | ComputeMemoryFlags.ReadWrite, velocities);
		ComputeBuffer<float> massBuffer = new ComputeBuffer<float>(kernel.Context, ComputeMemoryFlags.UseHostPointer | ComputeMemoryFlags.ReadWrite, masses);

		kernel.SetMemoryArgument(0, positionBuffer);
		kernel.SetMemoryArgument(1, velocityBuffer);
		kernel.SetMemoryArgument(2, massBuffer);
		kernel.SetValueArgument(3, Time.deltaTime);
		kernel.SetValueArgument(4, BodyAmount);
		
		// execute kernel
		queue.Execute(kernel, null, new long[] { BodyAmount }, null, null);

		// wait for completion
		queue.Finish();
		
		queue.ReadFromBuffer(velocityBuffer, ref velocities, false, null);

		for (int i = 0; i < positions.Length; i++)
			positions[i] = positions[i] + ( velocities[i] * Time.deltaTime );
		Debug.Log( positions[0] );
	}

	void SetUpCloo()
	{
		//Select GPU platform
		ComputePlatform platform = ComputePlatform.Platforms.First(p => p.Devices.Any(d => d.Type == ComputeDeviceTypes.Gpu));

		//Set the context to GPU
		ComputeContext context = new ComputeContext( ComputeDeviceTypes.Gpu, new ComputeContextPropertyList( platform ), null, IntPtr.Zero );

		//Create command queue
		queue = new ComputeCommandQueue( context, context.Devices[0], ComputeCommandQueueFlags.None );

		//Load Kernel
		string clSource = File.ReadAllText( Application.dataPath + "/kernel.cl" );
		//Debug.Log(clSource);

		//Create program from source
		ComputeProgram program = new ComputeProgram( context, clSource );

		//Compile source
		program.Build( null, null, null, IntPtr.Zero );
		
		//Load kernel
		kernel = program.CreateKernel( "affect" );
	}
}
