using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
namespace Destructible2D
{
	public class D2dCollider_Editor<T> : D2dEditor<T>
		where T : D2dCollider
	{
		protected override void OnInspector()
		{
			var updateColliderSettings = false;

			DrawDefault("IsTrigger", ref updateColliderSettings);
			DrawDefault("Material", ref updateColliderSettings);

			if (updateColliderSettings == true) DirtyEach(t => t.UpdateColliderSettings());
		}
	}
}
#endif

namespace Destructible2D
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(D2dDestructible))]
	public abstract class D2dCollider : MonoBehaviour
	{
		[Tooltip("Should these colliders be marked as triggers?")]
		public bool IsTrigger;

		[Tooltip("The physics material applied to these colliders")]
		public PhysicsMaterial2D Material;

		[SerializeField]
		protected GameObject child;

		[SerializeField]
		protected bool awoken;

		[System.NonSerialized]
		protected D2dDestructible destructible;

		// Set the 'IsTrigger' and 'Material' settings on all generated colliders
		public abstract void UpdateColliderSettings();

		[System.NonSerialized]
		private GameObject tempChild;

		[ContextMenu("Regenerate")]
		public void Regenerate()
		{
			OnAlphaDataReplaced();
		}

		public void DestroyChild()
		{
			if (child != null)
			{
				child = D2dHelper.Destroy(child);
			}
		}

		protected virtual void OnEnable()
		{
			if (destructible                     == null) destructible                     = GetComponent<D2dDestructible>();
			if (destructible.OnAlphaDataReplaced == null) destructible.OnAlphaDataReplaced = new D2dEvent();
			if (destructible.OnAlphaDataModified == null) destructible.OnAlphaDataModified = new D2dD2dRectEvent();
			if (destructible.OnAlphaDataSubset   == null) destructible.OnAlphaDataSubset   = new D2dD2dRectEvent();
			if (destructible.OnStartSplit        == null) destructible.OnStartSplit        = new D2dEvent();
			if (destructible.OnEndSplit          == null) destructible.OnEndSplit          = new D2dDestructibleListEvent();

			destructible.OnAlphaDataReplaced.AddListener(OnAlphaDataReplaced);
			destructible.OnAlphaDataModified.AddListener(OnAlphaDataModified);
			destructible.OnAlphaDataSubset  .AddListener(OnAlphaDataSubset);
			destructible.OnStartSplit       .AddListener(OnStartSplit);
			destructible.OnEndSplit         .AddListener(OnEndSplit);

			if (child != null)
			{
				child.SetActive(true);
			}
		}

		protected virtual void OnDisable()
		{
			destructible.OnAlphaDataReplaced.RemoveListener(OnAlphaDataReplaced);
			destructible.OnAlphaDataModified.RemoveListener(OnAlphaDataModified);
			destructible.OnAlphaDataSubset  .RemoveListener(OnAlphaDataSubset);
			destructible.OnStartSplit       .RemoveListener(OnStartSplit);
			destructible.OnEndSplit         .RemoveListener(OnEndSplit);

			if (child != null)
			{
				child.SetActive(false);
			}

			// If the collider was disabled while splitting then run this special code to destroy the children
			if (destructible.IsOnStartSplit == true)
			{
				if (child != null)
				{
					child.transform.SetParent(null, false);

					child = D2dHelper.Destroy(child);
				}

				if (tempChild != null)
				{
					tempChild = D2dHelper.Destroy(tempChild);
				}
			}
		}

		protected virtual void Awake()
		{
			// Auto destroy all default collider2Ds
			if (GetComponent<Collider2D>() != null)
			{
				var collider2Ds = GetComponents<Collider2D>();

				for (var i = collider2Ds.Length - 1; i >= 0; i--)
				{
					D2dHelper.Destroy(collider2Ds[i]);
				}
			}
		}

		protected virtual void Start()
		{
			if (awoken == false)
			{
				awoken = true;

				OnAlphaDataReplaced();
			}
		}

		protected virtual void Update()
		{
			if (child == null)
			{
				OnAlphaDataReplaced();
			}

			/*
			if (Input.GetKeyDown(KeyCode.Space))
			{
				var sw = System.Diagnostics.Stopwatch.StartNew();
				Regenerate();
				sw.Stop();
				Debug.Log(sw.ElapsedMilliseconds);
			}
			*/
		}

		protected virtual void OnDestroy()
		{
			DestroyChild();
		}

		protected virtual void OnAlphaDataReplaced()
		{
			UpdateBeforeBuild();
		}

		protected virtual void OnAlphaDataModified(D2dRect rect)
		{
			UpdateBeforeBuild();
		}

		protected virtual void OnAlphaDataSubset(D2dRect rect)
		{
			UpdateBeforeBuild();
		}

		protected virtual void OnStartSplit()
		{
			if (child != null)
			{
				child.transform.SetParent(null, false);

				tempChild = child;
				child     = null;
			}
		}

		protected virtual void OnEndSplit(List<D2dDestructible> clones)
		{
			ReconnectChild();
		}

		private void UpdateBeforeBuild()
		{
			if (destructible == null) destructible = GetComponent<D2dDestructible>();

			if (child == null)
			{
				ReconnectChild();

				if (child == null)
				{
					child = new GameObject("Collider");

					child.layer = transform.gameObject.layer;

					child.transform.SetParent(transform, false);
				}
			}

			if (destructible.AlphaIsValid == true)
			{
				var offsetX = destructible.AlphaRect.x;
				var offsetY = destructible.AlphaRect.y;
				var scaleX  = destructible.AlphaRect.width / destructible.AlphaWidth;
				var scaleY  = destructible.AlphaRect.height / destructible.AlphaHeight;

				child.transform.localPosition = new Vector3(offsetX, offsetY, 0.0f);
				child.transform.localScale    = new Vector3(scaleX, scaleY, 0.0f);
			}
		}

		private void ReconnectChild()
		{
			if (tempChild != null)
			{
				child = tempChild;

				child.transform.SetParent(transform, false);

				tempChild = null;
			}
		}
	}
}