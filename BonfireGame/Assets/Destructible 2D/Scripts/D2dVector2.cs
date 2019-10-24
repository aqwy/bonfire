namespace Destructible2D
{
	[System.Serializable]
	public struct D2dVector2
	{
		public int X;

		public int Y;

		public D2dVector2(int newX, int newY)
		{
			X = newX;
			Y = newY;
		}

		public static int DistanceSq(D2dVector2 a, D2dVector2 b)
		{
			var x = b.X - a.X;
			var y = b.Y - a.Y;

			return x * x + y * y;
		}

		public static D2dVector2 operator + (D2dVector2 a, D2dVector2 b)
		{
			a.X += b.X;
			a.Y += b.Y;

			return a;
		}

		public static D2dVector2 operator - (D2dVector2 a, D2dVector2 b)
		{
			a.X -= b.X;
			a.Y -= b.Y;

			return a;
		}

		public static D2dVector2 operator * (D2dVector2 a, float b)
		{
			a.X = (int)(a.X * b);
			a.Y = (int)(a.Y * b);

			return a;
		}

		public static D2dVector2 operator / (D2dVector2 a, int b)
		{
			a.X = a.X / b;
			a.Y = a.Y / b;

			return a;
		}

		public override bool Equals(System.Object o)
		{
			if (o is D2dVector2)
			{
				var v = (D2dVector2)o;

				return X == v.X && Y == v.Y;
			}

			return false;
		}

		public static bool operator ==(D2dVector2 a, D2dVector2 b)
		{
			return a.X == b.X && a.Y == b.Y;
		}

		public static bool operator !=(D2dVector2 a, D2dVector2 b)
		{
			return a.X != b.X || a.Y != b.Y;
		}

		public UnityEngine.Vector3 V
		{
			get
			{
				return new UnityEngine.Vector3(X, Y, 0.0f);
			}
		}

		public float Magnitude
		{
			get
			{
				return (float)System.Math.Sqrt(X * X + Y * Y);
			}
		}

		public override string ToString()
		{
			return " ("+ X + ", " + Y + ") ";
		}

		public override int GetHashCode()
		{
			return X.GetHashCode() ^ Y.GetHashCode();
		}
	}
}
