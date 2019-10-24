using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dSorter))]
	public class D2dSorter_Editor : D2dEditor<D2dSorter>
	{
		protected override void OnInspector()
		{
			EditorGUI.showMixedValue = Any(t => t.SortingOrder != Target.SortingOrder);
			
			var sortingOrder = Target.SortingOrder;
			
			EditorGUI.BeginChangeCheck();
			{
				sortingOrder = EditorGUILayout.IntField("Sorting Order", sortingOrder);
			}
			if (EditorGUI.EndChangeCheck() == true)
			{
				Each(t => t.SortingOrder = sortingOrder);
				
				MarkDirty();
			}
			
			var sortingLayerNames = GetSortingLayerNames();
			
			EditorGUI.showMixedValue = Any(t => t.SortingLayerID != Target.SortingLayerID);
			
			if (sortingLayerNames != null)
			{
				var index = System.Array.IndexOf(sortingLayerNames, Target.SortingLayerName);
				
				EditorGUI.BeginChangeCheck();
				{
					index = EditorGUILayout.Popup("Sorting Layer", index, sortingLayerNames);
				}
				if (EditorGUI.EndChangeCheck() == true && index >= 0)
				{
					var sortingLayerName = sortingLayerNames[index];
					
					Each(t => t.SortingLayerName = sortingLayerName);
					
					MarkDirty();
				}
			}
			else
			{
				var sortingLayerID = Target.SortingLayerID;
				
				EditorGUI.BeginChangeCheck();
				{
					sortingLayerID = EditorGUILayout.IntField("Sorting Layer", sortingLayerID);
				}
				if (EditorGUI.EndChangeCheck() == true)
				{
					Each(t => t.SortingLayerID = sortingLayerID);
					
					MarkDirty();
				}
			}
		}
		
		private string[] GetSortingLayerNames()
		{
			var sortingLayersProperty = typeof(UnityEditorInternal.InternalEditorUtility).GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
			
			if (sortingLayersProperty != null)
			{
				return (string[])sortingLayersProperty.GetValue(null, null);
			}
			
			return null;
		}
		
		private void MarkDirty()
		{
			Each(t => D2dHelper.SetDirty(t));
		}
	}
}
#endif

namespace Destructible2D
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Renderer))]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Sorter")]
	public class D2dSorter : MonoBehaviour
	{
		[System.NonSerialized]
		private Renderer tempRenderer;
		
		public int SortingOrder
		{
			set
			{
				if (tempRenderer == null) tempRenderer = GetComponent<Renderer>();
				
				tempRenderer.sortingOrder = value;
			}
			
			get
			{
				if (tempRenderer == null) tempRenderer = GetComponent<Renderer>();
				
				return tempRenderer.sortingOrder;
			}
		}
		
		public int SortingLayerID
		{
			set
			{
				if (tempRenderer == null) tempRenderer = GetComponent<Renderer>();
				
				tempRenderer.sortingLayerID = value;
			}
			
			get
			{
				if (tempRenderer == null) tempRenderer = GetComponent<Renderer>();
				
				return tempRenderer.sortingLayerID;
			}
		}
		
		public string SortingLayerName
		{
			set
			{
				if (tempRenderer == null) tempRenderer = GetComponent<Renderer>();
				
				tempRenderer.sortingLayerName = value;
			}
			
			get
			{
				if (tempRenderer == null) tempRenderer = GetComponent<Renderer>();
				
				return tempRenderer.sortingLayerName;
			}
		}
	}
}