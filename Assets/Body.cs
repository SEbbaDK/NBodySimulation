using UnityEngine;

public class Body
{
	public Vector3 Position, Velocity;
	public float Mass;

	public Body()
	{
		Position = Vector3.zero;
		Velocity = Vector3.zero;
	}

	public Body(Vector3 position, Vector3 velocity, float mass = 100)
	{
		Position = position;
		Velocity = velocity;
		Mass = mass;
	}
}
