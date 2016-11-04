using UnityEngine;

public class Body
{
	public Vector3 Position, Velocity;
	public float Mass;
	private static float G = 0.00000000006674f;

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

	public void AffectBy(Body otherBody, float amount)
	{
		Vector3 ABVector = otherBody.Position - Position;
		float r = ABVector.magnitude;
		float force = (G * (Mass * otherBody.Mass) / (r * r)) * amount;

		Velocity = Velocity + ((ABVector / r) * force);
	}
}
