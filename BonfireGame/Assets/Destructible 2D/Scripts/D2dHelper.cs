using UnityEngine;
using System.Collections.Generic;

namespace Destructible2D
{
	public static partial class D2dHelper
	{
		public const string ComponentMenuPrefix = "Destructible 2D/D2D ";

		public static byte[] AlphaData;

		private static float reciprocalOf255 = 1.0f / 255.0f;
		
		public static float ConvertAlpha(byte a)
		{
			return reciprocalOf255 * (float)a;
		}
		
		public static byte ConvertAlpha(float a)
		{
			return (byte)(255.0f * a);
		}
		
		public static T Destroy<T>(T o)
			where T : Object
		{
			if (o != null)
			{
#if UNITY_EDITOR
				if (Application.isPlaying == false)
				{
					Object.DestroyImmediate(o, true); return null;
				}
#endif
				Object.Destroy(o);
			}
			
			return null;
		}
		
		public static Matrix4x4 TranslationMatrix(Vector3 xyz)
		{
			return TranslationMatrix(xyz.x, xyz.y, xyz.z);
		}
		
		public static Matrix4x4 TranslationMatrix(float x, float y, float z)
		{
			var matrix = Matrix4x4.identity;
			
			matrix.m03 = x;
			matrix.m13 = y;
			matrix.m23 = z;
			
			return matrix;
		}
		
		public static Matrix4x4 RotationMatrix(Quaternion q)
		{
			var matrix = Matrix4x4.TRS(Vector3.zero, q, Vector3.one);
			
			return matrix;
		}
		
		public static Matrix4x4 ScalingMatrix(float xyz)
		{
			return ScalingMatrix(xyz, xyz, xyz);
		}
		
		public static Matrix4x4 ScalingMatrix(Vector3 xyz)
		{
			return ScalingMatrix(xyz.x, xyz.y, xyz.z);
		}
		
		public static Matrix4x4 ScalingMatrix(float x, float y, float z)
		{
			var matrix = Matrix4x4.identity;
			
			matrix.m00 = x;
			matrix.m11 = y;
			matrix.m22 = z;
			
			return matrix;
		}

		public static Vector3 ScreenToWorldPosition(Vector2 screenPosition, float intercept, Camera camera = null)
		{
			if (camera == null) camera = Camera.main;
			if (camera == null) return screenPosition;

			// Get ray of screen position
			var ray = camera.ScreenPointToRay(screenPosition);

			// Find point along this ray that intersects with Z = 0
			var distance = Divide(ray.origin.z - intercept, ray.direction.z);

			return ray.origin - ray.direction * distance;
		}
		
		public static bool IsValidUV(Vector2 uv)
		{
			return uv.x >= 0.0f && uv.y >= 0.0f && uv.x < 1.0f && uv.y < 1.0f;
		}

		public static bool IsValidUV(float u, float v)
		{
			return u >= 0.0f && v >= 0.0f && u < 1.0f && v < 1.0f;
		}
		
		public static bool Zero(float v)
		{
			return v == 0.0f;
		}
		
		public static float Reciprocal(float v)
		{
			return Zero(v) == false ? 1.0f / v : 0.0f;
		}
		
		public static float Divide(float a, float b)
		{
			return Zero(b) == false ? a / b : 0.0f;
		}
		
		public static float Atan2(Vector2 xy)
		{
			return Mathf.Atan2(xy.x, xy.y);
		}
		
		public static void Swap<T>(ref T a, ref T b)
		{
			var c = b;
			
			b = a;
			a = c;
		}
		
		public static bool AlphaIsValid(byte[] data, int width, int height)
		{
			return data != null && width > 0 && height > 0 && data.Length >= width * height;
		}

		public static bool AlphaIsValid(byte[] data, D2dRect rect)
		{
			return data != null && rect.IsSet == true && data.Length >= rect.Area;
		}

		public static float DampenFactor(float dampening, float elapsed)
		{
			return 1.0f - Mathf.Pow((float)System.Math.E, - dampening * elapsed);
		}
		
		public static float Dampen(float current, float target, float dampening, float elapsed, float minStep = 0.0f)
		{
			var factor   = DampenFactor(dampening, elapsed);
			var maxDelta = Mathf.Abs(target - current) * factor + minStep * elapsed;
			
			return Mathf.MoveTowards(current, target, maxDelta);
		}
		
		public static Vector2 Dampen2(Vector2 current, Vector2 target, float dampening, float elapsed, float minStep = 0.0f)
		{
			var factor   = DampenFactor(dampening, elapsed);
			var maxDelta = (target - current).magnitude * factor + minStep * elapsed;
			
			return Vector2.MoveTowards(current, target, maxDelta);
		}

		public static void ClearAlpha(int width, int height)
		{
			if (width <= 0 || height <= 0)
			{
				throw new System.ArgumentOutOfRangeException("Invalid width or height");
			}

			var total = width * height;

			// Replace alpha data array?
			if (AlphaData == null || AlphaData.Length != total)
			{
				AlphaData = new byte[total];
			}
			else
			{
				for (var i = 0; i < total; i++)
				{
					AlphaData[i] = 0;
                }
			}
		}

		public static void PasteAlpha(byte[] src, int srcWidth, int srcXMin, int srcXMax, int srcYMin, int srcYMax, int dstXMin, int dstYMin, int dstWidth)
		{
			for (var srcY = srcYMin; srcY < srcYMax; srcY++)
			{
				var dstOffset = (srcY - srcYMin + dstYMin) * dstWidth - srcXMin + dstXMin;
				var srcOffset = srcY * srcWidth;

				for (var srcX = srcXMin; srcX < srcXMax; srcX++)
				{
					var dstI = dstOffset + srcX;
					var srcI = srcOffset + srcX;

					AlphaData[dstI] = src[srcI];
				}
			}
		}

		public static bool ExtractAlpha(Texture2D texture2D, int x, int y, int width, int height)
		{
			if (texture2D != null && x >= 0 && y >= 0 && (x + width) <= texture2D.width && (y + height) <= texture2D.height)
			{
#if UNITY_EDITOR
				MakeTextureReadable(texture2D);
#endif
				var pixels32       = texture2D.GetPixels32();
				var total          = width * height;
				var texture2DWidth = texture2D.width;

				// Replace alpha data array?
				if (AlphaData == null || AlphaData.Length != total)
				{
					AlphaData = new byte[total];
				}

				// Copy alpha from texture
				for (var v = height - 1; v >= 0; v--)
				{
					var aOffset = v * width;
					var pOffset = (y + v) * texture2DWidth + x;

					for (var h = width - 1; h >= 0; h--)
					{
						AlphaData[aOffset + h] = pixels32[pOffset + h].a;
					}
				}

				return true;
			}

			return false;
		}

		public static void Halve(ref byte[] data, ref int width, ref int height)
		{
			if (data != null && data.Length >= width * height && width > 2 && height > 2)
			{
				var newWidth  = width / 2;
				var newHeight = height / 2;
				var newData   = new byte[newWidth * newHeight];
				var pixelW    = 1.0f / width;
				var pixelH    = 1.0f / height;
				var stepX     = (1.0f - pixelW * 2.0f) / (newWidth - 1);
				var stepY     = (1.0f - pixelH * 2.0f) / (newHeight - 1);

				for (var y = 0; y < newHeight; y++)
				{
					var newOffset = y * newWidth;

					for (var x = 0; x < newWidth; x++)
					{
						newData[x + newOffset] = GetBilinearFast(data, x * stepX + pixelW, y * stepY + pixelH, width, height);
					}
				}

				data   = newData;
				width  = newWidth;
				height = newHeight;
			}
		}

		public static void Blur(byte[] alphaData, int alphaWidth, int alphaHeight)
		{
			if (alphaData != null)
			{
				var alphaTotal = alphaWidth * alphaHeight;

				if (alphaData.Length >= alphaTotal)
				{
					if (AlphaData == null || AlphaData.Length < alphaTotal)
					{
						AlphaData = new byte[alphaTotal];
					}

					BlurHorizontally(alphaData, AlphaData, alphaWidth, alphaHeight);

					BlurVertically(AlphaData, alphaData, alphaWidth, alphaHeight);
				}
			}
		}

		private static void BlurHorizontally(byte[] src, byte[] dst, int width, int height)
		{
			for (var y = 0; y < height; y++)
			{
				for (var x = 0; x < width; x++)
				{
					var a = GetDefault(src, x - 1, y, width, height);
					var b = GetDefault(src, x, y, width);
					var c = GetDefault(src, x + 1, y, width, height);
					var t = (int)a + (int)b + (int)c;

					dst[x + y * width] = (byte)(t / 3);
				}
			}
		}

		private static void BlurVertically(byte[] src, byte[] dst, int width, int height)
		{
			for (var y = 0; y < height; y++)
			{
				for (var x = 0; x < width; x++)
				{
					var a = GetDefault(src, x, y - 1, width, height);
					var b = GetDefault(src, x, y, width);
					var c = GetDefault(src, x, y + 1, width, height);
					var t = (int)a + (int)b + (int)c;

					dst[x + y * width] = (byte)(t / 3);
				}
			}
		}

		public static byte GetDefault(byte[] data, int x, int y, int width)
		{
			return data[x + y * width];
		}

		public static byte GetDefault(byte[] data, int x, int y, int width, int height)
		{
			if (x >= 0 && x < width && y >= 0 && y < height)
			{
				return data[x + y * width];
			}

			return 0;
		}

		public static byte GetClamp(byte[] data, int x, int y, int width, int height)
		{
			if (x < 0) x = 0; else if (x >= width) x = width - 1;
			if (y < 0) y = 0; else if (y >= height) y = height - 1;

			return data[x + y * width];
		}

		public static byte GetBilinear(byte[] data, float u, float v, int width, int height)
		{
			u = u * (width - 1);
			v = v * (height - 1);

			var x = Mathf.FloorToInt(u);
			var y = Mathf.FloorToInt(v);
			var s = u - x;
			var t = v - y;

			var bl = GetClamp(data, x, y, width, height);
			var br = GetClamp(data, x + 1, y, width, height);
			var tl = GetClamp(data, x, y + 1, width, height);
			var tr = GetClamp(data, x + 1, y + 1, width, height);

			var bb = Lerp(bl, br, s);
			var tt = Lerp(tl, tr, s);

			return Lerp(bb, tt, t);
		}

		public static byte GetBilinearFast(byte[] data, float u, float v, int width, int height)
		{
			u = u * (width - 1);
			v = v * (height - 1);

			var x = Mathf.FloorToInt(u);
			var y = Mathf.FloorToInt(v);
			var s = u - x;
			var t = v - y;

			var bl = GetDefault(data, x, y, width);
			var br = GetDefault(data, x + 1, y, width);
			var tl = GetDefault(data, x, y + 1, width);
			var tr = GetDefault(data, x + 1, y + 1, width);

			var bb = Lerp(bl, br, s);
			var tt = Lerp(tl, tr, s);

			return Lerp(bb, tt, t);
		}

		public static byte Lerp(byte a, byte b, float t)
		{
			var i = 1.0f - t;

			return (byte)(a * i + b * t);
		}

		public static float Lerp(float a, float b, float t)
		{
			var i = 1.0f - t;

			return a * i + b * t;
		}

		public static float InverseLerp(float a, float b, float value)
		{
			if (a != b)
			{
				return (value - a) / (b - a);
			}

			return 0.0f;
		}

		public static bool CalculateRect(Matrix4x4 matrix, ref D2dRect rect, int sizeX, int sizeY)
		{
			// Grab transformed corners
			var a = matrix.MultiplyPoint(new Vector3(0.0f, 0.0f, 0.0f));
			var b = matrix.MultiplyPoint(new Vector3(1.0f, 0.0f, 0.0f));
			var c = matrix.MultiplyPoint(new Vector3(0.0f, 1.0f, 0.0f));
			var d = matrix.MultiplyPoint(new Vector3(1.0f, 1.0f, 0.0f));
			
			// Find min/max x/y
			var minX = Mathf.Min(Mathf.Min(a.x, b.x), Mathf.Min(c.x, d.x));
			var maxX = Mathf.Max(Mathf.Max(a.x, b.x), Mathf.Max(c.x, d.x));
			var minY = Mathf.Min(Mathf.Min(a.y, b.y), Mathf.Min(c.y, d.y));
			var maxY = Mathf.Max(Mathf.Max(a.y, b.y), Mathf.Max(c.y, d.y));
			
			// Has volume?
			if (minX < maxX && minY < maxY)
			{
				rect.MinX = Mathf.FloorToInt(minX * sizeX);
				rect.MaxX = Mathf. CeilToInt(maxX * sizeX);
				rect.MinY = Mathf.FloorToInt(minY * sizeY);
				rect.MaxY = Mathf. CeilToInt(maxY * sizeY);
				
				return true;
			}

			return false;
		}
	}
}