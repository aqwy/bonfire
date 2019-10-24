using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CustomEditor(typeof(D2dRequirements))]
	public class D2dRequirements_Editor : D2dEditor<D2dRequirements>
	{
		private bool showUnused;
		
		private bool showEvents;
		
		protected override void OnInspector()
		{
			showUnused = EditorGUI.Foldout(D2dHelper.Reserve(), showUnused, "Show Unused");
			
			DrawFloat("DamageMin", ref Target.UseDamageMin);
			DrawFloat("DamageMax", ref Target.UseDamageMax);
			DrawFloat("AlphaCountMin", ref Target.UseAlphaCountMin);
			DrawFloat("AlphaCountMax", ref Target.UseAlphaCountMax);
			DrawFloat("RemainingAlphaMin", ref Target.UseRemainingAlphaMin);
			DrawFloat("RemainingAlphaMax", ref Target.UseRemainingAlphaMax);
			
			EditorGUILayout.Separator();
			
			showEvents = EditorGUI.Foldout(D2dHelper.Reserve(), showEvents, "Show Events");
			
			if (showEvents == true)
			{
				DrawDefault("OnRequirementMet");
			}
		}
		
		private void DrawFloat(string title, ref bool use)
		{
			if (use == true || showUnused == true)
			{
				var rect  = D2dHelper.Reserve();
				var left  = rect; left.xMax -= 18.0f;
				var right = rect; right.xMin = right.xMax - 16.0f;
				
				EditorGUI.PropertyField(left, serializedObject.FindProperty(title));

				EditorGUI.BeginChangeCheck();
				{
					use = EditorGUI.Toggle(right, "", use);
				}
				if (EditorGUI.EndChangeCheck() == true)
				{
					D2dHelper.SetDirty(Target);
				}
			}
		}
		
		private void DrawFloat(string title, ref bool use, ref int value)
		{
			if (use == true || showUnused == true)
			{
				var rect  = D2dHelper.Reserve();
				var right = rect; right.xMin += EditorGUIUtility.labelWidth;
				var rect1 = right; rect1.xMax = rect1.xMin + 16.0f;
				var rect2 = right; rect2.xMin += 16.0f;
				
				EditorGUI.LabelField(rect, title);
				
				EditorGUI.BeginChangeCheck();
				{
					use   = EditorGUI.Toggle(rect1, "", use);
					value = EditorGUI.IntField(rect2, "", value);
				}
				if (EditorGUI.EndChangeCheck() == true)
				{
					D2dHelper.SetDirty(Target);
				}
			}
		}
	}
}
#endif

namespace Destructible2D
{
	[RequireComponent(typeof(D2dDestructible))]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Requirements")]
	public class D2dRequirements : MonoBehaviour
	{
		public bool UseDamageMin;
		
		[Tooltip("The minimum Damage required")]
		public float DamageMin;
		
		public bool UseDamageMax;
		
		[Tooltip("The maximum Damage required")]
		public float DamageMax;
		
		public bool UseAlphaCountMin;
		
		[Tooltip("The minimum AlphaCount required")]
		public int AlphaCountMin;
		
		public bool UseAlphaCountMax;
		
		[Tooltip("The maximum AlphaCount required")]
		public int AlphaCountMax;
		
		public bool UseRemainingAlphaMin;
		
		[Tooltip("The minimum RemainingAlpha required")]
		[Range(0.0f, 1.0f)]
		public float RemainingAlphaMin = 0.0f;
		
		public bool UseRemainingAlphaMax;
		
		[Tooltip("The maximum RemainingAlpha required")]
		[Range(0.0f, 1.0f)]
		public float RemainingAlphaMax = 1.0f;
		
		[Tooltip("This gets fired when all the requirements have been met")]
		public D2dEvent OnRequirementMet;
		
		[SerializeField]
		private bool met;
		
		[System.NonSerialized]
		private D2dDestructible destructible;
		
		public void UpdateMet()
		{
			if (met != CheckMet())
			{
				if (met == true)
				{
					met = false;
				}
				else
				{
					met = true;
					
					if (OnRequirementMet != null) OnRequirementMet.Invoke();
				}
			}
		}
		
		protected virtual void Update()
		{
			UpdateMet();
		}
		
		private bool CheckMet()
		{
			if (destructible == null) destructible = GetComponent<D2dDestructible>();
			
			if (UseDamageMin == true && destructible.Damage < DamageMin)
			{
				return false;
			}
			
			if (UseDamageMax == true && destructible.Damage > DamageMax)
			{
				return false;
			}
			
			if (UseAlphaCountMin == true && destructible.AlphaCount < AlphaCountMin)
			{
				return false;
			}
			
			if (UseAlphaCountMax == true && destructible.AlphaCount > AlphaCountMax)
			{
				return false;
			}
			
			if (UseRemainingAlphaMin == true && destructible.RemainingAlpha < RemainingAlphaMin)
			{
				return false;
			}
			
			if (UseRemainingAlphaMax == true && destructible.RemainingAlpha > RemainingAlphaMax)
			{
				return false;
			}
			
			return true;
		}
	}
}