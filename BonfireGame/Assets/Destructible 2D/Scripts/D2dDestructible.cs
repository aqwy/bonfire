using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dDestructible))]
	public class D2dDestructible_Editor : D2dEditor<D2dDestructible>
	{
		private bool showReplace;

		private bool showEvents;

		protected override void OnInspector()
		{
			var updateMesh = false;

			DrawDefault("Indestructible");
			DrawDefault("RecordAlphaCount");
			DrawDefault("AutoSharpen");

			Separator();

			DrawDefault("AutoSplit");

			if (Any(t => t.AutoSplit == true))
			{
				BeginIndent();
					DrawDefault("SplitExpand");
					BeginError(Any(t => t.SplitPixelsMin < 0));
						DrawDefault("SplitPixelsMin");
					EndError();
					BeginError(Any(t => t.SplitFeather < 0));
						DrawDefault("SplitFeather");
					EndError();
				EndIndent();
			}

			Separator();

			DrawDefault("MainTex");
			DrawDefault("DensityTex");
			DrawDefault("HealTex");
			DrawDefault("Sharpness");
			DrawDefault("color", ref updateMesh);

			DrawDamage();

			Separator();

			BeginDisabled(true);
			{
				BeginMixed(Any(t => t.AlphaTex != Target.AlphaTex));
					EditorGUI.ObjectField(D2dHelper.Reserve(), "Alpha Tex", Target.AlphaTex, typeof(Texture2D), false);
				EndMixed();
				BeginMixed(Any(t => t.AlphaCount != Target.AlphaCount));
					EditorGUI.IntField(D2dHelper.Reserve(), "Alpha Count", Target.AlphaCount);
				EndMixed();

				if (Targets.Length == 1 && Target.RecordAlphaCount == true)
				{
					EditorGUI.IntField(D2dHelper.Reserve(), "Original Alpha Count", Target.OriginalAlphaCount);
					EditorGUI.ProgressBar(D2dHelper.Reserve(), Target.RemainingAlpha, "Remaining Alpha");
				}
			}
			EndDisabled();

			Separator();

			showReplace = EditorGUI.Foldout(D2dHelper.Reserve(), showReplace, "Replace");

			if (showReplace == true)
			{
				DrawReplaceWith();
				DrawReplaceTextureWith();
				DrawReplaceAlphaWith();
			}

			Separator();

			showEvents = EditorGUI.Foldout(D2dHelper.Reserve(), showEvents, "Show Events");

			if (showEvents == true)
			{
				DrawDefault("OnStartSplit");
				DrawDefault("OnEndSplit");
				DrawDefault("OnDamageChanged");
				DrawDefault("OnAlphaDataReplaced");
				DrawDefault("OnAlphaDataModified");
				DrawDefault("OnAlphaDataSubset");
			}

			if (updateMesh == true) DirtyEach(t => t.UpdateMesh());
		}

		private void DrawDamage()
		{
			BeginMixed(Any(t => t.Damage != Target.Damage));
				var newDamage = EditorGUI.FloatField(D2dHelper.Reserve(), "Damage", Target.Damage);

				if (newDamage != Target.Damage)
				{
					Each(t => { t.Damage = newDamage; D2dHelper.SetDirty(t); });
				}
			EndMixed();
		}

		private void DrawReplaceWith()
		{
			var rect  = D2dHelper.Reserve();
			var right = rect; right.xMin += EditorGUIUtility.labelWidth;
			var rect1 = right; rect1.xMax -= rect1.width / 2;
			var rect2 = right; rect2.xMin += rect2.width / 2;

			EditorGUI.LabelField(rect, "With");

			var replaceSprite = (Sprite)EditorGUI.ObjectField(rect1, "", default(Object), typeof(Sprite), true);

			if (replaceSprite != null)
			{
				Each(t => { t.ReplaceWith(replaceSprite); D2dHelper.SetDirty(t); });
			}

			var replaceTexture2D = (Texture2D)EditorGUI.ObjectField(rect2, "", default(Object), typeof(Texture2D), true);

			if (replaceTexture2D != null)
			{
				Each(t => { t.ReplaceWith(replaceTexture2D); D2dHelper.SetDirty(t); });
			}
		}

		private void DrawReplaceTextureWith()
		{
			var rect  = D2dHelper.Reserve();
			var right = rect; right.xMin += EditorGUIUtility.labelWidth;
			var rect1 = right; rect1.xMax -= rect1.width / 2;
			var rect2 = right; rect2.xMin += rect2.width / 2;

			EditorGUI.LabelField(rect, "Texture With");

			var replaceSprite = (Sprite)EditorGUI.ObjectField(rect1, "", default(Object), typeof(Sprite), true);

			if (replaceSprite != null)
			{
				Each(t => { t.ReplaceTextureWith(replaceSprite); D2dHelper.SetDirty(t); });
			}

			var replaceTexture2D = (Texture2D)EditorGUI.ObjectField(rect2, "", default(Object), typeof(Texture2D), true);

			if (replaceTexture2D != null)
			{
				Each(t => { t.ReplaceTextureWith(replaceTexture2D); D2dHelper.SetDirty(t); });
			}
		}

		private void DrawReplaceAlphaWith()
		{
			var rect  = D2dHelper.Reserve();
			var right = rect; right.xMin += EditorGUIUtility.labelWidth;
			var rect1 = right; rect1.xMax -= rect1.width / 2;
			var rect2 = right; rect2.xMin += rect2.width / 2;

			EditorGUI.LabelField(rect, "Alpha With");

			var replaceSprite = (Sprite)EditorGUI.ObjectField(rect1, "", default(Object), typeof(Sprite), true);

			if (replaceSprite != null)
			{
				Each(t => { t.ReplaceAlphaWith(replaceSprite); D2dHelper.SetDirty(t); });
			}

			var replaceTexture2D = (Texture2D)EditorGUI.ObjectField(rect2, "", default(Object), typeof(Texture2D), true);

			if (replaceTexture2D != null)
			{
				Each(t => { t.ReplaceAlphaWith(replaceTexture2D); D2dHelper.SetDirty(t); });
			}
		}
	}
}
#endif

namespace Destructible2D
{
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Destructible")]
	public partial class D2dDestructible : MonoBehaviour
	{
		#region Static Fields
		// Stores all enabled destructibles in the current scene
		public static List<D2dDestructible> AllDestructibles = new List<D2dDestructible>();

		private static int[] indices = new int[] { 0, 2, 1, 3, 1, 2 };

		private static Vector3[] positions = new Vector3[4];

		private static Vector2[] coords = new Vector2[4];

		private static Color[] colors = new Color[4];

		private static MaterialPropertyBlock propertyBlock;

		private static List<D2dDestructible> clones = new List<D2dDestructible>();
		#endregion

		#region Fields
		// Called every time the alpha data is replaced/resized
		public D2dEvent OnAlphaDataReplaced;

		// Called every time you modify an area of the alpha data
		public D2dD2dRectEvent OnAlphaDataModified;

		// Called every time the alpha data is subset
		public D2dD2dRectEvent OnAlphaDataSubset;

		// Called every time the Damage field changes
		public D2dFloatFloatEvent OnDamageChanged;

		// Called on the original destructible before it gets split
		public D2dEvent OnStartSplit;

		// Called on the original destructible after it gets split
		public D2dDestructibleListEvent OnEndSplit;

		// Set while Split is being invoked, to prevent infinite cycles and such
		[System.NonSerialized]
		public bool IsSplitting;

		// Is OnStartSplit currently being invoked? (used to prevent collider generation issues)
		[System.NonSerialized]
		public bool IsOnStartSplit;

		[Tooltip("The main texture applied to the Destructible")]
		public Texture MainTex;

		[Tooltip("If you set this then areas with a brighter alpha will be harder to destroy")]
		public Texture2D DensityTex;

		[Tooltip("If you set this then this destructible can be healed up to the maximum opacity in this heal texture")]
		public Texture2D HealTex;

		[Tooltip("If you enable this then this Destructible will be unable to take damage")]
		public bool Indestructible;

		[Tooltip("If you enable this then the OriginalAlphaCount variable will automatically be calculated")]
		public bool RecordAlphaCount = true;

		[Tooltip("If you enable this then the sharpness will automatically get modified by how detailed the Alpha Tex is relative to the Main Tex")]
		public bool AutoSharpen = true;

		[Tooltip("If you enable this then the destructible will automatically split pixel islands (areas in the Alpha Tex that aren't connected by solid pixels)")]
		public bool AutoSplit;

		[Tooltip("The minimum amount of pixels required in an island for it to be split")]
		public int SplitPixelsMin = 5;

		[Tooltip("The amount of pixels around a stamp that will be checked for splits (-1 = whole Alpha Tex)")]
		public int SplitExpand = -1;

		[Tooltip("Should the split islands get an extra border of pixels added to keep any original antialiasing?")]
		public int SplitFeather = 1;

		[Tooltip("Allows you to set how sharp the edges of the Destructible image are")]
		[Range(1.0f, 5.0f)]
		public float Sharpness = 1.0f;

		// Make this private so we can detect when it changes via property and update the material
		[SerializeField]
		[Tooltip("The color tint of this destructible object")]
		private Color color = Color.white;

		[SerializeField]
		[Tooltip("The amount of numerical damage this destructible has taken (this value is separate from any visual damage)")]
		private float damage;

		[SerializeField]
		private Rect textureRect;

		[SerializeField]
		private Rect originalRect;

		[SerializeField]
		private Rect alphaRect;

		[SerializeField]
		private int alphaWidth;

		[SerializeField]
		private int alphaHeight;

		[SerializeField]
		private int alphaCount = -1;

		[SerializeField]
		private int originalAlphaCount = -1;

		[SerializeField]
		private byte[] alphaData;

		[System.NonSerialized]
		private Texture2D alphaTex;

		[System.NonSerialized]
		private D2dRect alphaDirty;

		[System.NonSerialized]
		private D2dRect alphaModified;

		//[System.NonSerialized]
		[SerializeField]
		private Vector2 alphaScale;

		//[System.NonSerialized]
		[SerializeField]
		private Vector2 alphaOffset;

		[System.NonSerialized]
		private float alphaRatio;

		[System.NonSerialized]
		private Mesh mesh;

		[System.NonSerialized]
		private MeshRenderer meshRenderer;

		[System.NonSerialized]
		private MeshFilter meshFilter;
		#endregion

		#region Properties
		public Color Color
		{
			set
			{
				if (color != value)
				{
					color = value;

					if (mesh != null)
					{
						UpdateMeshColors();
					}
				}
			}

			get
			{
				return color;
			}
		}

		public float Damage
		{
			set
			{
				if (damage != value && Indestructible == false)
				{
					var old = damage;

					damage = value;

					if (OnDamageChanged != null) OnDamageChanged.Invoke(old, value);
				}
			}

			get
			{
				return damage;
			}
		}

		public Texture2D AlphaTex
		{
			get
			{
				DeserializeAlphaTex();

				return alphaTex;
			}
		}

		public int AlphaWidth
		{
			get
			{
				return alphaWidth;
			}
		}

		public int AlphaHeight
		{
			get
			{
				return alphaHeight;
			}
		}

		public int AlphaCount
		{
			get
			{
				if (alphaCount == -1)
				{
					alphaCount = 0;

					var total = alphaWidth * alphaHeight;

					for (var i = 0; i < total; i++)
					{
						if (alphaData[i] > 127)
						{
							alphaCount += 1;
						}
					}
				}

				return alphaCount;
			}
		}

		public int OriginalAlphaCount
		{
			set
			{
				originalAlphaCount = value;
			}

			get
			{
				// Recalculate? This shouldn't be done here really
				if (originalAlphaCount == -1)
				{
					originalAlphaCount = AlphaCount;
				}

				return originalAlphaCount;
			}
		}

		public float RemainingAlpha
		{
			get
			{
				return D2dHelper.Divide(AlphaCount, OriginalAlphaCount);
			}
		}

		public byte[] AlphaData
		{
			get
			{
				return alphaData;
			}
		}

		public bool AlphaIsValid
		{
			get
			{
				return D2dHelper.AlphaIsValid(alphaData, alphaWidth, alphaHeight);
			}
		}

		public Rect AlphaRect
		{
			get
			{
				return alphaRect;
			}
		}

		public Rect TextureRect
		{
			get
			{
				return textureRect;
			}
		}

		public Rect OriginalRect
		{
			get
			{
				return originalRect;
			}
		}

		public Matrix4x4 AlphaToWorldMatrix
		{
			get
			{
				var matrix1 = D2dHelper.ScalingMatrix(alphaRect.width, alphaRect.height, 1.0f);
				var matrix2 = D2dHelper.TranslationMatrix(alphaRect.x, alphaRect.y, 0.0f);
				
				return transform.localToWorldMatrix * matrix2 * matrix1;
			}
		}

		public Matrix4x4 WorldToAlphaMatrix
		{
			get
			{
				/*
				var invX    = D2dHelper.Reciprocal(alphaRect.width);
				var invY    = D2dHelper.Reciprocal(alphaRect.height);
				var matrix1 = D2dHelper.ScalingMatrix(invX, invY, 1.0f);
				var matrix2 = D2dHelper.TranslationMatrix(-alphaRect.x, -alphaRect.y, 0.0f);
				
				return matrix1 * matrix2 * transform.worldToLocalMatrix;
				*/
				var matrix1 = Matrix4x4.identity;
				var matrix2 = Matrix4x4.identity;

				matrix1.m00 = D2dHelper.Reciprocal(alphaRect.width);
				matrix1.m11 = D2dHelper.Reciprocal(alphaRect.height);

				matrix2.m03 = -alphaRect.x;
				matrix2.m13 = -alphaRect.y;

				return matrix1 * matrix2 * transform.worldToLocalMatrix;
			}
		}
		#endregion

		public void SetIndestructible(bool newIndestructible)
		{
			Indestructible = newIndestructible;
		}

		public void Clear()
		{
			ClearAlpha();
		}

		public void ClearAlpha()
		{
			alphaData   = null;
			alphaWidth  = 0;
			alphaHeight = 0;
			alphaCount  = 0;
			alphaTex    = D2dHelper.Destroy(alphaTex);
		}

		public float SampleAlpha(Vector3 worldPosition)
		{
			var uv = (Vector2)WorldToAlphaMatrix.MultiplyPoint(worldPosition);

			if (D2dHelper.IsValidUV(uv) == true)
			{
				var x = Mathf.FloorToInt(uv.x * alphaWidth );
				var y = Mathf.FloorToInt(uv.y * alphaHeight);

				return SampleAlpha(x + y * alphaWidth);
			}

			return 0.0f;
		}

		public static D2dHit RaycastAlphaFirst(Vector3 startPosition, Vector3 endPosition)
		{
			var bestDistance = float.PositiveInfinity;
			var bestHit      = default(D2dHit);

			for (var i = AllDestructibles.Count - 1; i >= 0; i--)
			{
				var destructible = AllDestructibles[i];
				var hit          = destructible.RaycastAlpha(startPosition, endPosition);

				if (hit != null && hit.Distance < bestDistance)
				{
					bestDistance = hit.Distance;
					bestHit      = hit;
				}
			}

			return bestHit;
		}

		public D2dHit RaycastAlpha(Vector3 positionStart, Vector3 positionEnd)
		{
			var pointStart = WorldToAlphaMatrix.MultiplyPoint(positionStart);
			var pointEnd   = WorldToAlphaMatrix.MultiplyPoint(positionEnd  );
			var pixelStart = default(D2dVector2);
			var pixelEnd   = default(D2dVector2);

			pixelStart.X = Mathf.RoundToInt(pointStart.x * alphaWidth );
			pixelStart.Y = Mathf.RoundToInt(pointStart.y * alphaHeight);
			pixelEnd.X   = Mathf.RoundToInt(pointEnd.x * alphaWidth );
			pixelEnd.Y   = Mathf.RoundToInt(pointEnd.y * alphaHeight);

			return RaycastAlpha(pixelStart, pixelEnd);
		}

		public D2dHit RaycastAlpha(D2dVector2 start, D2dVector2 end)
		{
			var bounds    = default(Bounds);
			var distance  = default(float);
			var direction = (end - start).V.normalized;

			// Clip start and end points to bounds
			bounds.SetMinMax(new Vector3(0.0f, 0.0f, -1.0f), new Vector3(alphaWidth, alphaHeight, 1.0f));

			if (start != end && bounds.IntersectRay(new Ray(start.V, direction), out distance) == true)
			{
				if (distance > 0)
				{
					start.X = Mathf.RoundToInt(start.X + direction.x * distance);
					start.Y = Mathf.RoundToInt(start.Y + direction.y * distance);
				}

				if (start != end && bounds.IntersectRay(new Ray(end.V, -direction), out distance) == true)
				{
					if (distance > 0)
					{
						end.X = Mathf.RoundToInt(end.X - direction.x * distance);
						end.Y = Mathf.RoundToInt(end.Y - direction.y * distance);
					}

					// Sample all pixels across line
					var width  = end.X - start.X;
					var height = end.Y - start.Y;
					var dx1    = 0;
					var dy1    = 0;
					var dx2    = 0;
					var dy2    = 0;

					if (width  < 0) dx1 = -1; else if (width  > 0) dx1 = 1;
					if (height < 0) dy1 = -1; else if (height > 0) dy1 = 1;
					if (width  < 0) dx2 = -1; else if (width  > 0) dx2 = 1;

					var longest  = Mathf.Abs(width);
					var shortest = Mathf.Abs(height);

					if (longest <= shortest)
					{
						longest  = Mathf.Abs(height);
						shortest = Mathf.Abs(width);
						dx2      = 0;

						if (height < 0) dy2 = -1; else if (height > 0) dy2 = 1;
					}

					var x         = start.X;
					var y         = start.Y;
					var numerator = longest >> 1;

					for (int i = 0; i <= longest; i++)
					{
						if (SampleAlpha(x, y) >= 0.5f)
						{
							var hit = new D2dHit();

							hit.Pixel.X  = x;
							hit.Pixel.Y  = y;
							hit.Point.x  = (float)x / (float)alphaWidth;
							hit.Point.y  = (float)y / (float)alphaHeight;
							hit.Position = AlphaToWorldMatrix.MultiplyPoint(hit.Point);
							hit.Distance = Vector2.Distance(new Vector2(x, y), new Vector2(start.X, start.Y));

							return hit;
						}

						numerator += shortest;

						if (!(numerator < longest))
						{
							numerator -= longest;
							x         += dx1;
							y         += dy1;
						}
						else
						{
							x += dx2;
							y += dy2;
						}
					}
				}
			}

			return null;
		}

		public static Matrix4x4 CalculateSliceMatrix(Vector2 startPos, Vector2 endPos, float thickness)
		{
			var mid   = (startPos + endPos) / 2.0f;
			var vec   = endPos - startPos;
			var size  = new Vector2(thickness, vec.magnitude);
			var angle = D2dHelper.Atan2(vec) * -Mathf.Rad2Deg;

			return CalculateStampMatrix(mid, size, angle);
		}

		public static Matrix4x4 CalculateStampMatrix(Vector2 position, Vector2 size, float angle)
		{
			var t = D2dHelper.TranslationMatrix(position.x, position.y, 0.0f);
			var r = D2dHelper.RotationMatrix(Quaternion.Euler(0.0f, 0.0f, angle));
			var s = D2dHelper.ScalingMatrix(size.x, size.y, 1.0f);
			var o = D2dHelper.TranslationMatrix(-0.5f, -0.5f, 0.0f); // Centre stamp

			return t * r * s * o;
		}

		public static void SliceAll(Vector2 startPos, Vector2 endPos, float thickness, Texture2D stampTex, float hardness, int layerMask = -1)
		{
			StampAll(CalculateSliceMatrix(startPos, endPos, thickness), stampTex, hardness, layerMask);
		}

		public static void StampAll(Vector2 position, Vector2 size, float angle, Texture2D stampTex, float hardness, int layerMask = -1)
		{
			StampAll(CalculateStampMatrix(position, size, angle), stampTex, hardness, layerMask);
		}

		public static void StampAll(Matrix4x4 matrix, Texture2D stampTex, float hardness, int layerMask = -1)
		{
			for (var i = AllDestructibles.Count - 1; i >= 0; i--)
			{
				var destructible = AllDestructibles[i];

				if (destructible != null && destructible.Indestructible == false)
				{
					var mask = 1 << destructible.gameObject.layer;

					if ((layerMask & mask) != 0)
					{
						destructible.BeginAlphaModifications();
						{
							destructible.Stamp(matrix, stampTex, hardness);
						}
						destructible.EndAlphaModifications();
					}
				}
			}
		}

		public void Slice(Vector2 startPos, Vector2 endPos, float thickness, Texture2D stampTex, float hardness)
		{
			Stamp(CalculateSliceMatrix(startPos, endPos, thickness), stampTex, hardness);
		}

		public void Stamp(Vector2 position, Vector2 size, float angle, Texture2D stampTex, float hardness)
		{
			Stamp(CalculateStampMatrix(position, size, angle), stampTex, hardness);
		}
		
		public D2dRect OriginalRectRelative
		{
			get
			{
				var originalW = D2dHelper.Divide(originalRect.width , alphaRect.width ) * alphaWidth;
				var originalH = D2dHelper.Divide(originalRect.height, alphaRect.height) * alphaHeight;
				var x         = Mathf.CeilToInt((originalRect.x - alphaRect.x) * originalW);
				var y         = Mathf.CeilToInt((originalRect.y - alphaRect.y) * originalH);
				var w         = Mathf.FloorToInt(originalW);
				var h         = Mathf.FloorToInt(originalH);
				
				return D2dRect.CreateFromMinSize(x, y, w, h);
			}
		}

		public void AddDamage(int amount)
		{
			Damage += amount;
		}

		public void RemoveDamage(int amount)
		{
			Damage -= amount;
		}

		public void Stamp(Matrix4x4 stampMatrix, Texture2D stampTex, float hardness)
		{
			if (AlphaIsValid == false)
			{
				throw new System.InvalidOperationException("Invalid alpha");
			}

			if (stampTex == null)
			{
				return;
			}

			if (hardness == 0.0f)
			{
				return;
			}

			if (hardness < 0.0f && HealTex == null)
			{
				return; // Does healing without a HealTex make sense?
			}
#if UNITY_EDITOR
			D2dHelper.MakeTextureReadable(stampTex);
			D2dHelper.MakeTextureReadable(DensityTex);
			D2dHelper.MakeTextureReadable(HealTex);
#endif
			var matrix = WorldToAlphaMatrix * stampMatrix;
			var rect   = default(D2dRect);

			if (D2dHelper.CalculateRect(matrix, ref rect, alphaWidth, alphaHeight) == true)
			{
				/*
				// Expand alpha data so we can add new pixels?
				if (hardness < 0.0f)
				{
					// Make sure we don't expand past the original rect's size
					rect = D2dRect.CalculateOverlap(rect, OriginalRectRelative);
					
					var oldRect = D2dRect.CreateFromMinSize(0, 0, alphaWidth, alphaHeight);
					var newRect = D2dRect.CalculateCombined(oldRect, rect);
					
					if (newRect.Area != oldRect.Area)
					{
						D2dHelper.ClearAlpha(newRect.SizeX, newRect.SizeY);

						var offX = oldRect.MinX - newRect.MinX;
						var offY = oldRect.MinY - newRect.MinY;

						D2dHelper.PasteAlpha(alphaData, alphaWidth, oldRect.MinX, oldRect.MaxX, oldRect.MinY, oldRect.MaxY, offX, offY, newRect.SizeX);

						SubsetAlphaWith(D2dHelper.AlphaData, newRect, alphaCount);
					}
				}
				*/
				rect.MinX = Mathf.Clamp(rect.MinX, 0, alphaWidth );
				rect.MaxX = Mathf.Clamp(rect.MaxX, 0, alphaWidth );
				rect.MinY = Mathf.Clamp(rect.MinY, 0, alphaHeight);
				rect.MaxY = Mathf.Clamp(rect.MaxY, 0, alphaHeight);

				var inverse         = matrix.inverse;
				var alphaPixelX     = D2dHelper.Reciprocal(alphaWidth);
				var alphaPixelY     = D2dHelper.Reciprocal(alphaHeight);
				var alphaHalfPixelX = alphaPixelX * 0.5f;
				var alphaHalfPixelY = alphaPixelY * 0.5f;

				// Heal?
				if (hardness < 0.0f)
				{
					if (HealTex != null)
					{
						var healOffsetX = (alphaRect.x - originalRect.x) / originalRect.width;
						var healOffsetY = (alphaRect.y - originalRect.y) / originalRect.height;
						var healScaleX  = alphaRect.width / originalRect.width;
						var healScaleY  = alphaRect.height / originalRect.height;

						for (var y = rect.MinY; y < rect.MaxY; y++)
						{
							var v = y * alphaPixelY + alphaHalfPixelY;

							for (var x = rect.MinX; x < rect.MaxX; x++)
							{
								var u       = x * alphaPixelX + alphaHalfPixelX;
								var stampUV = inverse.MultiplyPoint(new Vector3(u, v, 0.0f));

								if (D2dHelper.IsValidUV(stampUV) == true)
								{
									var healU = u * healScaleX + healOffsetX;
									var healV = v * healScaleY + healOffsetY;

									if (D2dHelper.IsValidUV(healU, healV) == true)
									{
										var index = x + y * alphaWidth;
										var alpha = SampleAlpha(index);
										var stamp = SampleStamp(stampTex, stampUV) * hardness;
										var heal  = SampleHeal(healU, healV);
										
										WriteAlpha(index, Mathf.Min(alpha - stamp, heal));
									}
								}
							}
						}
					}
					else
					{
						for (var y = rect.MinY; y < rect.MaxY; y++)
						{
							var v = y * alphaPixelY + alphaHalfPixelY;

							for (var x = rect.MinX; x < rect.MaxX; x++)
							{
								var u       = x * alphaPixelX + alphaHalfPixelX;
								var stampUV = inverse.MultiplyPoint(new Vector3(u, v, 0.0f));

								if (D2dHelper.IsValidUV(stampUV) == true)
								{
									var index = x + y * alphaWidth;
									var alpha = SampleAlpha(index);
									var stamp = SampleStamp(stampTex, stampUV) * hardness;

									WriteAlpha(index, alpha - stamp);
								}
							}
						}
					}
				}
				// Damage?
				else
				{
					if (DensityTex != null)
					{
						var densityOffsetX = (alphaRect.x - originalRect.x) / originalRect.width;
						var densityOffsetY = (alphaRect.y - originalRect.y) / originalRect.height;
						var densityScaleX  = alphaRect.width / originalRect.width;
						var densityScaleY  = alphaRect.height / originalRect.height;

						for (var y = rect.MinY; y < rect.MaxY; y++)
						{
							var v = y * alphaPixelY + alphaHalfPixelY;

							for (var x = rect.MinX; x < rect.MaxX; x++)
							{
								var u       = x * alphaPixelX + alphaHalfPixelX;
								var stampUV = inverse.MultiplyPoint(new Vector3(u, v, 0.0f));

								if (D2dHelper.IsValidUV(stampUV) == true)
								{
									var densityU = u * densityScaleX + densityOffsetX;
									var densityV = v * densityScaleY + densityOffsetY;

									if (D2dHelper.IsValidUV(densityU, densityV) == true)
									{
										var index   = x + y * alphaWidth;
										var alpha   = SampleAlpha(index);
										var stamp   = SampleStamp(stampTex, stampUV) * hardness;
										var density = SampleDensity(densityU, densityV);

										if (stamp > density)
										{
											WriteAlpha(index, alpha - (stamp - density));
										}
									}
								}
							}
						}
					}
					else
					{
						for (var y = rect.MinY; y < rect.MaxY; y++)
						{
							var v = y * alphaPixelY + alphaHalfPixelY;

							for (var x = rect.MinX; x < rect.MaxX; x++)
							{
								var u       = x * alphaPixelX + alphaHalfPixelX;
								var stampUV = inverse.MultiplyPoint(new Vector3(u, v, 0.0f));

								if (D2dHelper.IsValidUV(stampUV) == true)
								{
									var index = x + y * alphaWidth;
									var alpha = SampleAlpha(index);
									var stamp = SampleStamp(stampTex, stampUV) * hardness;

									WriteAlpha(index, alpha - stamp);
								}
							}
						}
					}
				}

				alphaModified.Add(rect);
			}
		}

		public void WriteAlpha(int x, int y, byte alpha)
		{
			if (x >= 0 && y >= 0 && x < alphaWidth && y < alphaHeight)
			{
				alphaModified.Add(x, y);

				alphaData[x + y * alphaWidth] = alpha;
			}
		}
		
		public void BeginAlphaModifications()
		{
			alphaModified.Clear();
		}

		public void EndAlphaModifications()
		{
			if (alphaModified.IsSet == true)
			{
				// Add the currently modified alpha rect to the dirty texture rect
				alphaDirty.Add(alphaModified);

				// Alpha count is no longer known
				alphaCount = -1;

				// Spliting causes the destructibles to get subset, so don't raise this event if it was split
				if (AutoSplit == true)
				{
					var rect = alphaModified;

					if (SplitExpand < 0)
					{
						rect.Expand(Mathf.Max(alphaWidth, alphaHeight));
					}
					else
					{
						rect.Expand(SplitExpand);
					}

					if (TrySplit(rect) == false)
					{
						if (OnAlphaDataModified != null) OnAlphaDataModified.Invoke(alphaModified);
					}
				}
				else
				{
					if (OnAlphaDataModified != null) OnAlphaDataModified.Invoke(alphaModified);
				}
			}
		}

		public float SampleAlpha(int x, int y)
		{
			if (x >= 0 && y >= 0 && x < alphaWidth && y < alphaHeight)
			{
				return SampleAlpha(x + y * alphaWidth);
            }

			return 0.0f;
		}

		private float SampleAlpha(int i)
		{
			return D2dHelper.ConvertAlpha(alphaData[i]);
		}

		private void WriteAlpha(int i, float alpha)
		{
			alphaData[i] = D2dHelper.ConvertAlpha(Mathf.Clamp01(alpha));
		}

		private float SampleStamp(Texture2D texture2D, Vector2 uv)
		{
			var x = (int)(uv.x * texture2D.width);
			var y = (int)(uv.y * texture2D.height);

			return texture2D.GetPixel(x, y).a;
		}

		private float SampleDensity(float u, float v)
		{
			var x = (int)(u * DensityTex.width );
			var y = (int)(v * DensityTex.height);

			return DensityTex.GetPixel(x, y).a;
		}

		private float SampleHeal(float u, float v)
		{
			var x = (int)(u * HealTex.width );
			var y = (int)(v * HealTex.height);

			return HealTex.GetPixel(x, y).a;
		}

		public void ReplaceWith(Sprite sprite)
		{
			if (sprite != null)
			{
				var texture2D = sprite.texture;

				if (texture2D != null)
				{
					var scale = 1.0f / sprite.pixelsPerUnit;

					// Resize original rect
					originalRect.x      = (-sprite.pivot.x + Mathf.Ceil(sprite.textureRectOffset.x)) * scale;
					originalRect.y      = (-sprite.pivot.y + Mathf.Ceil(sprite.textureRectOffset.y)) * scale;
					originalRect.width  = Mathf.Floor(sprite.textureRect.width) * scale;
					originalRect.height = Mathf.Floor(sprite.textureRect.height) * scale;

					// Copy texture
					MainTex = texture2D;

					// Initial alpha rect matches original rect
					alphaRect = originalRect;

					// Fill alpha data
					ReplaceAlphaWith(sprite);

					UpdateMesh();
				}
			}
		}

		public void ReplaceWith(Texture2D texture2D)
		{
			if (texture2D != null)
			{
				// Resize original rect
				originalRect.x      = texture2D.width * -0.5f;
				originalRect.y      = texture2D.height * -0.5f;
				originalRect.width  = texture2D.width;
				originalRect.height = texture2D.height;

				// Copy texture
				MainTex = texture2D;

				// Initial alpha rect matches original rect
				alphaRect = originalRect;

				// Fill alpha data
				ReplaceAlphaWith(texture2D);

				UpdateMesh();
			}
		}

		public void ReplaceTextureWith(Texture2D texture2D)
		{
			if (texture2D != null)
			{
				MainTex = texture2D;
			}
		}

		public void ReplaceTextureWith(Sprite sprite)
		{
			if (sprite != null)
			{
				var texture2D = sprite.texture;

				if (texture2D != null)
				{
					var scale   = 1.0f / sprite.pixelsPerUnit;
					//var oldRect = originalRect;

					// Resize original rect
					originalRect.x      = (-sprite.pivot.x + Mathf.Ceil(sprite.textureRectOffset.x)) * scale;
					originalRect.y      = (-sprite.pivot.y + Mathf.Ceil(sprite.textureRectOffset.y)) * scale;
					originalRect.width  = Mathf.Floor(sprite.textureRect.width) * scale;
					originalRect.height = Mathf.Floor(sprite.textureRect.height) * scale;

					// Copy texture
					MainTex = texture2D;

					var texRect   = sprite.textureRect;
					var texX      = Mathf.CeilToInt(texRect.x);
					var texY      = Mathf.CeilToInt(texRect.y);
					var texWidth  = Mathf.FloorToInt(texRect.width);
					var texHeight = Mathf.FloorToInt(texRect.height);

					textureRect.x      = D2dHelper.Divide(texX, texture2D.width);
					textureRect.y      = D2dHelper.Divide(texY, texture2D.height);
					textureRect.width  = D2dHelper.Divide(texWidth, texture2D.width);
					textureRect.height = D2dHelper.Divide(texHeight, texture2D.height);

					/*
					

					// Initial alpha rect matches original rect
					alphaRect = originalRect;
					rect
					
					*/
					UpdateMesh();
				}
			}
		}

		public void ReplaceAlphaWith(D2dSnapshot snapshot)
		{
			if (snapshot != null)
			{
				alphaRect = snapshot.AlphaRect;

				ReplaceAlphaWith(snapshot.AlphaData, snapshot.AlphaWidth, snapshot.AlphaHeight);
			}
		}

		public void ReplaceAlphaWith(Sprite sprite)
		{
			if (sprite != null)
			{
				var texture2D = sprite.texture;

				if (texture2D != null)
				{
					var texRect   = sprite.textureRect;
					var texX      = Mathf.CeilToInt(texRect.x);
					var texY      = Mathf.CeilToInt(texRect.y);
					var texWidth  = Mathf.FloorToInt(texRect.width);
					var texHeight = Mathf.FloorToInt(texRect.height);

					ReplaceAlphaWith(texture2D, texX, texY, texWidth, texHeight);
				}
			}
		}

		public void ReplaceAlphaWith(Texture2D texture2D)
		{
			if (texture2D != null)
			{
				ReplaceAlphaWith(texture2D, 0, 0, texture2D.width, texture2D.height);
			}
		}

		public void ReplaceAlphaWith(Texture2D texture2D, int x, int y, int width, int height, int newAlphaCount = -1)
		{
			if (D2dHelper.ExtractAlpha(texture2D, x, y, width, height) == true)
			{
				// Write texture rect
				textureRect.x      = D2dHelper.Divide(x, texture2D.width);
				textureRect.y      = D2dHelper.Divide(y, texture2D.height);
				textureRect.width  = D2dHelper.Divide(width, texture2D.width);
				textureRect.height = D2dHelper.Divide(height, texture2D.height);

				ReplaceAlphaWith(D2dHelper.AlphaData, width, height, newAlphaCount);
			}
		}

		public void ReplaceAlphaWith(byte[] newAlphaData, int newAlphaWidth, int newAlphaHeight, int newAlphaCount = -1)
		{
			if (newAlphaData != null && newAlphaWidth > 0 && newAlphaHeight > 0)
			{
				var newAlphaTotal = newAlphaWidth * newAlphaHeight;

				if (newAlphaData.Length >= newAlphaTotal)
				{
					FastCopyAlphaData(newAlphaData, newAlphaWidth, newAlphaHeight, newAlphaCount);

					if (RecordAlphaCount == true)
					{
						originalAlphaCount = AlphaCount;
					}
					else
					{
						originalAlphaCount = -1;
					}

					alphaDirty.Set(0, newAlphaWidth, 0, newAlphaHeight);

					if (OnAlphaDataReplaced != null) OnAlphaDataReplaced.Invoke();
				}
			}
		}

		// Replace a specific area of the current alphdaData
		public void UpdateAlpha(byte[] newAlphaData, D2dRect newAlphaRect)
		{
			if (AlphaIsValid == true)
			{
				if (D2dHelper.AlphaIsValid(newAlphaData, newAlphaRect) == true)
				{
					var oldRect = new D2dRect(0, alphaWidth, 0, alphaHeight);
					var lapRect = D2dRect.CalculateOverlap(oldRect, newAlphaRect);

					if (lapRect.IsSet == true)
					{
						for (var y = lapRect.MinY; y < lapRect.MaxY; y++)
						{
							//var oy = y * AlphaWidth;

							for (var x = lapRect.MinX; x < lapRect.MaxX; x++)
							{
								var nx = x - newAlphaRect.MinX;
								var ny = y - newAlphaRect.MinY;
								var na = newAlphaData[ny * newAlphaRect.SizeX + nx];

								WriteAlpha(x, y, na);
							}
						}

						alphaModified.Set(lapRect.MinX, lapRect.MaxX, lapRect.MinY, lapRect.MaxY);

						alphaDirty.Add(alphaModified);

						if (OnAlphaDataModified != null) OnAlphaDataModified.Invoke(alphaModified);
					}
				}
				else
				{
					Debug.LogError("New alpha data is null or invalid size " + newAlphaData.Length + " - " + newAlphaRect.SizeX + " - " + newAlphaRect.SizeY);
				}
			}
			else
			{
				Debug.LogError("Alpha is not valid");
			}
		}
		
		private void FastCopyAlphaData(byte[] newAlphaData, int newAlphaWidth, int newAlphaHeight, int newAlphaCount = -1)
		{
			var newAlphaTotal = newAlphaWidth * newAlphaHeight;

			if (alphaData == null || alphaData.Length != newAlphaTotal)
			{
				alphaData = new byte[newAlphaTotal];
			}

			for (var i = newAlphaTotal - 1; i >= 0; i--)
			{
				alphaData[i] = newAlphaData[i];
			}

			alphaWidth  = newAlphaWidth;
			alphaHeight = newAlphaHeight;
			alphaCount  = newAlphaCount;
			alphaRatio  = 0.0f;
		}

		// This returns a snapshot of the current destruction state
		// Use ReplaceAlphaWith to revert to this snapshot
		public D2dSnapshot GetSnapshot(D2dSnapshot snapshot = null)
		{
			if (AlphaIsValid == true)
			{
				if (snapshot == null) snapshot = new D2dSnapshot();

				var alphaTotal = alphaWidth * alphaHeight;

				if (snapshot.AlphaData == null || snapshot.AlphaData.Length <= alphaTotal)
				{
					snapshot.AlphaData = new byte[alphaTotal];
				}

				for (var i = 0; i < alphaTotal; i++)
				{
					snapshot.AlphaData[i] = alphaData[i];
				}

				snapshot.AlphaWidth  = alphaWidth;
				snapshot.AlphaHeight = alphaHeight;
				snapshot.AlphaRect   = alphaRect;

				return snapshot;
			}

			return null;
		}

		// This automatically trims, blurs, and halves the alpha (if you're doing this from code then you should do these operations separately, as they can be redundant)
		[ContextMenu("Optimize Alpha")]
		public void OptimizeAlpha()
		{
			TrimAlpha();

			BlurAlpha(false);
			HalveAlpha(true);

			TrimAlpha();
		}

		[ContextMenu("Halve Alpha")]
		public void HalveAlpha()
		{
#if UNITY_EDITOR
			D2dHelper.SetDirty(this);
#endif
			HalveAlpha(true);
		}

		public void HalveAlpha(bool replace)
		{
			var oldWidth  = alphaWidth;
			var oldHeight = alphaHeight;

			D2dHelper.Halve(ref alphaData, ref alphaWidth, ref alphaHeight);
			
			var pixelW = D2dHelper.Reciprocal(oldWidth ) * alphaRect.width;
			var pixelH = D2dHelper.Reciprocal(oldHeight) * alphaRect.height;

			alphaRect.xMin += pixelW * 0.5f;
			alphaRect.xMax -= pixelW * 0.5f;
			alphaRect.yMin += pixelH * 0.5f;
			alphaRect.yMax -= pixelH * 0.5f;

			if (replace == true)
			{
				ReplaceAlphaWith(alphaData, alphaWidth, alphaHeight);
			}
		}

		[ContextMenu("Blur Alpha")]
		public void BlurAlpha()
		{
#if UNITY_EDITOR
			D2dHelper.SetDirty(this);
#endif
			BlurAlpha(true);
		}

		public void BlurAlpha(bool replace)
		{
			D2dHelper.Blur(alphaData, alphaWidth, alphaHeight);

			if (replace == true)
			{
				ReplaceAlphaWith(alphaData, alphaWidth, alphaHeight);
			}
		}

		// This will find the min/max of the current alpha and subset it with a border, removing any hard edges
		[ContextMenu("Trim Alpha")]
		public void TrimAlpha()
		{
			if (AlphaIsValid == false)
			{
				throw new System.InvalidOperationException("Invalid alpha");
			}
#if UNITY_EDITOR
			D2dHelper.SetDirty(this);
#endif
			var xMin = 0;
			var xMax = alphaWidth;
			var yMin = 0;
			var yMax = alphaHeight;

			for (var x = xMin; x < xMax; x++)
			{
				if (FastSolidAlphaVertical(yMin, yMax, x) == false) xMin += 1; else break;
			}

			for (var x = xMax - 1; x >= xMin; x--)
			{
				if (FastSolidAlphaVertical(yMin, yMax, x) == false) xMax -= 1; else break;
			}

			for (var y = yMin; y < yMax; y++)
			{
				if (FastSolidAlphaHorizontal(xMin, xMax, y) == false) yMin += 1; else break;
			}

			for (var y = yMax - 1; y >= yMin; y--)
			{
				if (FastSolidAlphaHorizontal(xMin, xMax, y) == false) yMax -= 1; else break;
			}

			var width  = xMax - xMin + 2;
			var height = yMax - yMin + 2;
			var rect   = D2dRect.CreateFromMinSize(xMin - 1, yMin - 1, width, height);

			D2dHelper.ClearAlpha(width, height);

			D2dHelper.PasteAlpha(alphaData, alphaWidth, xMin, xMax, yMin, yMax, 1, 1, width);

			SubsetAlphaWith(D2dHelper.AlphaData, rect, alphaCount);
		}

		private bool FastSolidAlphaHorizontal(int xMin, int xMax, int y)
		{
			var offset = y * alphaWidth;

            for (var x = xMin; x < xMax; x++)
			{
				if (alphaData[x + offset] > 0)
				{
					return true;
				}
			}

			return false;
		}

		private bool FastSolidAlphaVertical(int yMin, int yMax, int x)
		{
			for (var y = yMin; y < yMax; y++)
			{
                if (alphaData[x + y * alphaWidth] > 0)
				{
					return true;
				}
			}

			return false;
		}

		// This resets the alpha data with the data from the original texture
		[ContextMenu("Reset Alpha")]
		public void ResetAlpha()
		{
			var texture2D = MainTex as Texture2D;

			if (texture2D != null)
			{
				var x = Mathf.RoundToInt(textureRect.x      * MainTex.width);
				var y = Mathf.RoundToInt(textureRect.y      * MainTex.height);
				var w = Mathf.RoundToInt(textureRect.width  * MainTex.width);
				var h = Mathf.RoundToInt(textureRect.height * MainTex.height);

				alphaRect = originalRect;

				UpdateMesh();

				ReplaceAlphaWith(texture2D, x, y, w, h);
			}
		}

		// This will split the current object even if 'Splittable' is none
		[ContextMenu("Try Split")]
		public bool TrySplit()
		{
			var rect = new D2dRect();

			rect.MaxX = alphaWidth;
			rect.MaxY = alphaHeight;

			return TrySplit(rect);
		}

		private static D2dDistanceField distanceField = new D2dDistanceField();

		// This will split the current object even if 'Splittable' is none
		public bool TrySplit(D2dRect splitRect)
		{
			if (IsSplitting == true)
			{
				return false;
			}

			if (AlphaIsValid == false)
			{
				throw new System.InvalidOperationException("Invalid alpha");
			}

			splitRect.ClampTo(0, alphaWidth, 0, alphaHeight);

			if (splitRect.IsSet == false)
			{
				throw new System.ArgumentOutOfRangeException("Split rect does not overlap alpha");
			}

			D2dFloodfill.FastFind(alphaData, alphaWidth, alphaHeight, splitRect);

			for (var i = D2dFloodfill.IslandsA.Count - 1; i >= 0; i--)
			{
				var island = D2dFloodfill.IslandsA[i];

				if (island.Count < SplitPixelsMin)
				{
				}
			}

			D2dSplitGroup.ClearAll();

			//Debug.Log(D2dFloodfill.IslandLayout + " - " + D2dFloodfill.IslandsA.Count + " - " + D2dFloodfill.IslandsB.Count + " - " + D2dFloodfill.IslandsC.Count);

			switch (D2dFloodfill.IslandLayout)
			{
				case D2dFloodfill.Layout.Whole:
				{
					if (D2dFloodfill.IslandsA.Count > 1)
					{
						var baseRect = splitRect; baseRect.Expand(SplitFeather); baseRect.ClampTo(0, alphaWidth, 0, alphaHeight);

						distanceField.Transform(baseRect, alphaWidth, alphaHeight, alphaData);

						for (var i = D2dFloodfill.IslandsA.Count - 1; i >= 0; i--)
						{
							var island     = D2dFloodfill.IslandsA[i];
							var islandRect = new D2dRect(island.MinX, island.MaxX, island.MinY, island.MaxY); islandRect.Expand(SplitFeather); islandRect.ClampTo(splitRect);
							var splitGroup = D2dSplitGroup.GetSplitGroup();

							island.Submit(distanceField, splitGroup, baseRect, islandRect);
						}

						SplitWhole(D2dSplitGroup.SplitGroups);

						return true;
					}
				}
				break;

				case D2dFloodfill.Layout.Island:
				{
					Debug.Log("This split type hasn't been implemented yet, please use Split Expand = -1");
					/*
					if (D2dFloodfill.IslandsA.Count + D2dFloodfill.IslandsB.Count > 1)
					{
						var baseRect = splitRect; baseRect.Expand(SplitFeather); baseRect.ClampTo(0, alphaWidth, 0, alphaHeight);

						distanceField.Transform(baseRect, alphaWidth, alphaHeight, alphaData);

						for (var i = D2dFloodfill.IslandsA.Count - 1; i >= 0; i--)
						{
							var island     = D2dFloodfill.IslandsA[i];
							var islandRect = new D2dRect(island.MinX, island.MaxX, island.MinY, island.MaxY); islandRect.Expand(SplitFeather); islandRect.ClampTo(splitRect);
							var splitGroup = D2dSplitGroup.GetSplitGroup();

							island.Submit(distanceField, splitGroup, baseRect, islandRect);
						}

						SplitWhole(D2dSplitGroup.SplitGroups);

						return true;
					}
					*/
				}
				break;

				case D2dFloodfill.Layout.Horizontal:
				{
					Debug.Log("This split type hasn't been implemented yet, please use Split Expand = -1");
				}
				break;

				case D2dFloodfill.Layout.Vertical:
				{
					Debug.Log("This split type hasn't been implemented yet, please use Split Expand = -1");
				}
				break;
			}

			return false;
		}

		// This allows you to split the current destructible into multiple smaller pieces.
		// If you only want to chip parts off the original then store the modified original data in the borderGroup
		public void SplitWhole(List<D2dSplitGroup> groups)
		{
			if (groups == null || groups.Count < 2)
			{
				return;
			}

			IsSplitting = true; clones.Clear();
			{
				IsOnStartSplit = true;
				{
					if (OnStartSplit != null) OnStartSplit.Invoke();
				}
				IsOnStartSplit = false;

				var originalData   = alphaData;
				var originalWidth  = alphaWidth;
				var originalHeight = alphaHeight;

				// Null data so it doesn't get cloned
				alphaData = null;

				for (var i = groups.Count - 1; i >= 0; i--)
				{
					var group = groups[i];
					var clone = i == 0 ? this : CloneForSplit();

					group.GenerateData();

					group.CombineData(originalData, originalWidth, originalHeight);

					clone.SubsetAlphaWith(group.Data, group.Rect);

					clones.Add(clone);
				}

				if (OnEndSplit != null) OnEndSplit.Invoke(clones);
			}
			IsSplitting = false; clones.Clear();
		}

		private D2dDestructible CloneForSplit()
		{
			var clone = Instantiate(this);

			// Retain name and tag
			clone.name = name;
			clone.tag  = tag;

			// Retain layer
			clone.gameObject.layer = gameObject.layer;

			// Retain transform
			clone.transform.SetParent(transform.parent, false);

			clone.transform.localPosition = transform.localPosition;
			clone.transform.localRotation = transform.localRotation;
			clone.transform.localScale    = transform.localScale;

			return clone;
		}

		[ContextMenu("Update Mesh")]
		public void UpdateMesh()
		{
			if (meshFilter == null) meshFilter = gameObject.GetComponent<MeshFilter>();
			if (meshFilter == null) meshFilter = gameObject.AddComponent<MeshFilter>();

			if (MainTex != null)
			{
				var paddedTextureRect = textureRect;

				FastPadRect(ref paddedTextureRect);

				if (mesh == null)
				{
					mesh = new Mesh();

					UpdateMeshData(paddedTextureRect);

					mesh.hideFlags = HideFlags.DontSave;
					mesh.name      = "Destructible Mesh";
					mesh.triangles = indices;
				}
				else
				{
					UpdateMeshData(paddedTextureRect);
				}

				UpdateProperties(paddedTextureRect);

				meshFilter.sharedMesh = mesh;
			}
			else
			{
				meshFilter.sharedMesh = null;
			}
		}

		public void SubsetAlphaWith(byte[] subData, D2dRect subRect, int newAlphaCount = -1)
		{
			if (D2dHelper.AlphaIsValid(subData, subRect) == false)
			{
				throw new System.ArgumentException("Invalid subset data");
			}
			
			var stepX = D2dHelper.Divide(alphaRect.width , alphaWidth );
			var stepY = D2dHelper.Divide(alphaRect.height, alphaHeight);

			alphaRect.x      += stepX * subRect.MinX;
			alphaRect.y      += stepY * subRect.MinY;
			alphaRect.width  += stepX * (subRect.SizeX - alphaWidth );
			alphaRect.height += stepY * (subRect.SizeY - alphaHeight);

			FastCopyAlphaData(subData, subRect.SizeX, subRect.SizeY, newAlphaCount);

			UpdateMesh();

			alphaDirty.Set(0, subRect.SizeX, 0, subRect.SizeY);

			if (OnAlphaDataSubset != null) OnAlphaDataSubset.Invoke(subRect);
		}

		protected virtual void Awake()
		{
			var spriteRenderer = GetComponent<SpriteRenderer>();

			if (spriteRenderer != null)
			{
				// grab data from renderer
				var sprite         = spriteRenderer.sprite;
				var sortingOrder   = spriteRenderer.sortingOrder;
				var sortingLayerID = spriteRenderer.sortingLayerID;
				var material       = spriteRenderer.sharedMaterial;

				// Destroy renderer
				DestroyImmediate(spriteRenderer);

				// Replace
				ReplaceWith(sprite);

				UpdateMesh();
				UpdateRenderer(material);

				// Add sorter
				var sorter = gameObject.AddComponent<D2dSorter>();

				sorter.SortingOrder   = sortingOrder;
				sorter.SortingLayerID = sortingLayerID;
			}
			else
			{
				UpdateMesh();
				UpdateRenderer(null);
			}
		}

		protected virtual void OnEnable()
		{
			AllDestructibles.Add(this);
		}

		protected virtual void OnDisable()
		{
			AllDestructibles.Remove(this);
		}

		protected virtual void OnWillRenderObject()
		{
			DeserializeAlphaTex();

			if (MainTex != null && alphaTex != null)
			{
				if (meshRenderer  == null) meshRenderer  = gameObject.GetComponent<MeshRenderer>();
				if (meshRenderer  == null) meshRenderer  = gameObject.AddComponent<MeshRenderer>();
				if (propertyBlock == null) propertyBlock = new MaterialPropertyBlock();

				var alphaSharpness = Sharpness;

				if (AutoSharpen == true)
				{
					// Recalculate?
                    if (alphaRatio <= 0.0f)
					{
						var textureWidth = D2dHelper.Divide(alphaRect.width, originalRect.width) * MainTex.width * textureRect.width;

						alphaRatio = D2dHelper.Divide(textureWidth, alphaWidth);
					}

					alphaSharpness *= alphaRatio;
				}

				propertyBlock.SetTexture("_MainTex", MainTex);
				propertyBlock.SetTexture("_AlphaTex", alphaTex);
				propertyBlock.SetVector("_AlphaScale", alphaScale);
				propertyBlock.SetVector("_AlphaOffset", alphaOffset);
				propertyBlock.SetFloat("_AlphaSharpness", alphaSharpness);

				meshRenderer.SetPropertyBlock(propertyBlock);
			}
		}

#if UNITY_EDITOR
		protected virtual void OnDrawGizmosSelected()
		{
			Gizmos.matrix = transform.localToWorldMatrix;

			Gizmos.DrawWireCube(alphaRect.center, alphaRect.size);

			Gizmos.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);

			Gizmos.DrawWireCube(originalRect.center, originalRect.size);
		}
#endif
		private void UpdateRenderer(Material oldMaterial)
		{
			if (meshRenderer == null) meshRenderer = gameObject.GetComponent<MeshRenderer>();
			if (meshRenderer == null) meshRenderer = gameObject.AddComponent<MeshRenderer>();

			if (meshRenderer.sharedMaterial == null)
			{
				/*
				if (oldMaterial != null)
				{
					var oldShader = oldMaterial.shader;
					
					if (oldShader.name == "Standard")
					{
						meshRenderer.sharedMaterial = Resources.Load<Material>("Destructible 2D/Standard"); return;
					}
				}
				*/
				meshRenderer.sharedMaterial = Resources.Load<Material>("Destructible 2D/Default");
			}
		}
		
		// This will automatically scale a textureRect to account for automatic NPOT upscaling
		private void FastPadRect(ref Rect rect)
		{
			if (SystemInfo.npotSupport == NPOTSupport.Full)
			{
				return;
			}

			if (SystemInfo.npotSupport == NPOTSupport.Restricted)
			{
				var texture2D = MainTex as Texture2D;

				if (texture2D != null && texture2D.mipmapCount <= 1)
				{
					return;
				}
			}

			var width     = MainTex.width;
			var height    = MainTex.height;
			var padWidth  = Mathf.NextPowerOfTwo(width );
			var padHeight = Mathf.NextPowerOfTwo(height);

			if (width != padWidth)
			{
				var scale = width / (float)padWidth;

				rect.x     *= scale;
				rect.width *= scale;
			}

			if (height != padHeight)
			{
				var scale = height / (float)padHeight;

				rect.y      *= scale;
				rect.height *= scale;
			}
		}

		private static readonly Vector3[] quadNormals = new Vector3[] { new Vector3(0.0f, 0.0f, -1.0f), new Vector3(0.0f, 0.0f, -1.0f), new Vector3(0.0f, 0.0f, -1.0f), new Vector3(0.0f, 0.0f, -1.0f) };

		private static readonly Vector4[] quadTangents = new Vector4[] { new Vector4(1.0f, 0.0f, 0.0f, -1.0f), new Vector4(1.0f, 0.0f, 0.0f, -1.0f), new Vector4(1.0f, 0.0f, 0.0f, -1.0f), new Vector4(1.0f, 0.0f, 0.0f, -1.0f) };

		private void UpdateMeshData(Rect paddedTextureRect)
		{
			positions[0] = new Vector3(alphaRect.xMin, alphaRect.yMin, 0.0f);
			positions[1] = new Vector3(alphaRect.xMax, alphaRect.yMin, 0.0f);
			positions[2] = new Vector3(alphaRect.xMin, alphaRect.yMax, 0.0f);
			positions[3] = new Vector3(alphaRect.xMax, alphaRect.yMax, 0.0f);

			mesh.vertices = positions;
			
			UpdateMeshColors();
			
			UpdateMeshCoords(paddedTextureRect);
			
			mesh.normals  = quadNormals;
			mesh.tangents = quadTangents;
			
			mesh.RecalculateBounds();
		}
		
		private void UpdateMeshColors()
		{
			colors[0] = Color;
			colors[1] = Color;
			colors[2] = Color;
			colors[3] = Color;

			mesh.colors = colors;
		}

		private void UpdateMeshCoords(Rect paddedTextureRect)
		{
			var l = D2dHelper.InverseLerp(originalRect.xMin, originalRect.xMax, alphaRect.xMin);
			var r = D2dHelper.InverseLerp(originalRect.xMin, originalRect.xMax, alphaRect.xMax);
			var b = D2dHelper.InverseLerp(originalRect.yMin, originalRect.yMax, alphaRect.yMin);
			var t = D2dHelper.InverseLerp(originalRect.yMin, originalRect.yMax, alphaRect.yMax);

			var xMin = paddedTextureRect.x + paddedTextureRect.width  * l;
			var xMax = paddedTextureRect.x + paddedTextureRect.width  * r;
			var yMin = paddedTextureRect.y + paddedTextureRect.height * b;
			var yMax = paddedTextureRect.y + paddedTextureRect.height * t;

			coords[0] = new Vector2(xMin, yMin);
			coords[1] = new Vector2(xMax, yMin);
			coords[2] = new Vector2(xMin, yMax);
			coords[3] = new Vector2(xMax, yMax);

			mesh.uv = coords;
		}

		private void UpdateProperties(Rect paddedTextureRect)
		{
			alphaScale.x = D2dHelper.Divide(D2dHelper.Divide(originalRect.width , alphaRect.width ), paddedTextureRect.width );
			alphaScale.y = D2dHelper.Divide(D2dHelper.Divide(originalRect.height, alphaRect.height), paddedTextureRect.height);

			alphaOffset.x = paddedTextureRect.x + paddedTextureRect.width  * D2dHelper.Divide(alphaRect.x - originalRect.x, originalRect.width );
			alphaOffset.y = paddedTextureRect.y + paddedTextureRect.height * D2dHelper.Divide(alphaRect.y - originalRect.y, originalRect.height);
		}

		private void DeserializeAlphaTex()
		{
			if (AlphaIsValid == true)
			{
				// Make from scratch?
				if (alphaTex == null)
				{
					ConstructAlphaTex();
				}
				// Destroy and make from scratch?
				else if (alphaTex.width != alphaWidth || alphaTex.height != alphaHeight)
				{
					alphaTex = D2dHelper.Destroy(alphaTex);

					ConstructAlphaTex();
				}
				// Update area?
				else if (alphaDirty.IsSet == true)
				{
					ReconstructAlphaTex();
				}
			}
			else
			{
				Clear();
			}
		}

		private void ConstructAlphaTex()
		{
			alphaTex = new Texture2D(alphaWidth, alphaHeight, TextureFormat.Alpha8, false);

			alphaTex.hideFlags = HideFlags.DontSave;
			alphaTex.wrapMode  = TextureWrapMode.Clamp;

			for (var y = 0; y < alphaHeight; y++)
			{
				for (var x = 0; x < alphaWidth; x++)
				{
					var color = default(Color);
					var alpha = alphaData[x + y * alphaWidth];

					color.a = D2dHelper.ConvertAlpha(alpha);

					alphaTex.SetPixel(x, y, color);
				}
			}

			alphaTex.Apply();

			alphaDirty.Clear();
		}

		private void ReconstructAlphaTex()
		{
			var xMin = alphaDirty.MinX;
			var xMax = alphaDirty.MaxX;
			var yMin = alphaDirty.MinY;
			var yMax = alphaDirty.MaxY;

			for (var y = yMin; y < yMax; y++)
			{
				for (var x = xMin; x < xMax; x++)
				{
					var color = default(Color);
					var alpha = alphaData[x + y * alphaWidth];

					color.a = D2dHelper.ConvertAlpha(alpha);

					alphaTex.SetPixel(x, y, color);
				}
			}

			alphaTex.Apply();

			alphaDirty.Clear();
		}
	}
}
