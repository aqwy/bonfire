using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dAutoCollider))]
	public class D2dAutoCollider_Editor : D2dCollider_Editor<D2dAutoCollider>
	{
		protected override void OnInspector()
		{
			base.OnInspector();
		}
	}
}
#endif

namespace Destructible2D
{
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Auto Collider")]
	public class D2dAutoCollider : D2dCollider
	{
		[SerializeField]
		private PolygonCollider2D polygonCollider2D;
		
		public override void UpdateColliderSettings()
		{
			if (polygonCollider2D != null)
			{
				polygonCollider2D.isTrigger      = IsTrigger;
				polygonCollider2D.sharedMaterial = Material;
			}
		}
		
		protected override void OnAlphaDataReplaced()
		{
			base.OnAlphaDataReplaced();
			
			Rebuild();
		}
		
		protected override void OnAlphaDataModified(D2dRect rect)
		{
			base.OnAlphaDataModified(rect);
			
			Rebuild();
		}
		
		protected override void OnAlphaDataSubset(D2dRect rect)
		{
			base.OnAlphaDataSubset(rect);
			
			Rebuild();
		}
		
		protected override void OnStartSplit()
		{
			base.OnStartSplit();
			
			polygonCollider2D = null;
		}
		
		private void Destroy()
		{
			polygonCollider2D = D2dHelper.Destroy(polygonCollider2D);
		}
		
		private void Rebuild()
		{
			Destroy();
			
			var alphaTex = destructible.AlphaTex;
			
			if (alphaTex != null)
			{
				var spriteRenderer = child.GetComponent<SpriteRenderer>();
				var sprite         = Sprite.Create(alphaTex, new Rect(0, 0, alphaTex.width, alphaTex.height), Vector2.zero, 1.0f, 0, SpriteMeshType.FullRect);
				
				if (spriteRenderer == null)
				{
					spriteRenderer = child.AddComponent<SpriteRenderer>();
				}
				
				spriteRenderer.sprite = sprite;
				
				polygonCollider2D = child.AddComponent<PolygonCollider2D>();
				
				polygonCollider2D.enabled        = IsDefaultPolygonCollider2D(polygonCollider2D) == false;
				polygonCollider2D.isTrigger      = IsTrigger;
				polygonCollider2D.sharedMaterial = Material;
				
				D2dHelper.Destroy(sprite);
				D2dHelper.Destroy(spriteRenderer);
			}
		}
		
		// The default collider is a pentagon, but its position and size changes based on the sprite
		private static bool IsDefaultPolygonCollider2D(PolygonCollider2D polygonCollider2D)
		{
			if (polygonCollider2D == null) return false;
			
			if (polygonCollider2D.GetTotalPointCount() != 5) return false;
			
			var points  = polygonCollider2D.points;
			var spacing = Vector2.Distance(points[0], points[4]);
			
			// Same spacing?
			for (var i = 0; i < 4; i++)
			{
				var spacing2 = Vector2.Distance(points[i], points[i + 1]);
				
				if (Mathf.Approximately(spacing, spacing2) == false)
				{
					return false;
				}
			}
			
			var midpoint = (points[0] + points[1] + points[2] + points[3] + points[4]) * 0.2f;
			var radius   = Vector2.Distance(points[0], midpoint);
			
			// Same radius?
			for (var i = 1; i < 5; i++)
			{
				var radius2 = Vector2.Distance(points[i], midpoint);
				
				if (Mathf.Approximately(radius, radius2) == false)
				{
					return false;
				}
			}
			
			// Must be a pentagon then!
			return true;
		}
	}
}