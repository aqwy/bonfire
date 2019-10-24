using UnityEngine;

namespace Destructible2D
{
	// This class stores a snapshot of a Destructible's current state of destruction
	[System.Serializable]
	public class D2dSnapshot
	{
		public Rect AlphaRect;
		
		public byte[] AlphaData;
		
		public int AlphaWidth;
		
		public int AlphaHeight;
	}
}