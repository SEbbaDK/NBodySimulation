using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using Cloo;
using Random = UnityEngine.Random;
using Debug = UnityEngine.Debug;

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
		SetUpCloo();

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

	void SetUpCloo()
	{
		//Select GPU platform
		ComputePlatform platform = ComputePlatform.Platforms[1]; //Needs to select the gpu

		//Set the context to GPU
		ComputeContext context = new ComputeContext( ComputeDeviceTypes.Gpu, new ComputeContextPropertyList( platform ), null, IntPtr.Zero );

		//Create command queue
		ComputeCommandQueue queue = new ComputeCommandQueue( context, context.Devices[0], ComputeCommandQueueFlags.None );

		//Load Kernel
		string clSource = File.ReadAllText( Application.dataPath + "/kernel.cl" );

		//Create program from source
		ComputeProgram program = new ComputeProgram( context, clSource );

		//Compile source
		program.Build( null, null, null, IntPtr.Zero );

		//Load kernel
		ComputeKernel kernel = program.CreateKernel( "helloWorld" );

		//TEMP
		// create a ten integer array and its length
		int[] message = { 1, 2, 3, 4, 5 };
		int messageSize = message.Length;

		// allocate a memory buffer with the message (the int array)
		ComputeBuffer<int> messageBuffer = new ComputeBuffer<int>(context,
			ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.UseHostPointer, message);

		kernel.SetMemoryArgument(0, messageBuffer); // set the integer array
		kernel.SetValueArgument(1, messageSize); // set the array size

		// execute kernel
		queue.ExecuteTask(kernel, null);

		// wait for completion
		queue.Finish();
	}
}
