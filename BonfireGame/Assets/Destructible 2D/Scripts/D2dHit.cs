using UnityEngine;

namespace Destructible2D
{
	// This stores information about a raycast hit from the D2dDestructible class
	[System.Serializable]
	public class D2dHit
	{
		public D2dVector2 Pixel;

		public Vector2 Point;

		public Vector3 Position;

		public float Distance;
	}
}
