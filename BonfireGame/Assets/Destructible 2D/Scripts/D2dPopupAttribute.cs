using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Destructible2D
{
	public class D2dPopupAttribute : PropertyAttribute
	{
		public GUIContent[] Names;
		
		public int[] Values;
		
		public D2dPopupAttribute(params int[] newValues)
		{
			Names = newValues.Select(v => new GUIContent(v.ToString())).ToArray();
			
			Values = newValues.Select(v => v).ToArray();
		}
	}
	
#if UNITY_EDITOR
	[CustomPropertyDrawer(typeof(D2dPopupAttribute))]
	public class D2dPopupDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var Attribute = (D2dPopupAttribute)attribute;
			
			EditorGUI.IntPopup(position, property, Attribute.Names, Attribute.Values, label);
		}
	}
#endif
}