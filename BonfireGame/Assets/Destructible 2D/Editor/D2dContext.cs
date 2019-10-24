using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

namespace Destructible2D
{
	public static class D2dContext
	{
		public const string ContextMenuPrefix = "New - ";
		
		[UnityEditor.MenuItem("CONTEXT/SpriteRenderer/" + ContextMenuPrefix + "Make Destructible", true)]
		private static bool MakeDestructibleValidate(MenuCommand menuCommand)
		{
			return AddSingleComponentValidate<D2dDestructible>(GetGameObject(menuCommand));
		}
		
		[UnityEditor.MenuItem("CONTEXT/SpriteRenderer/" + ContextMenuPrefix + "Make Destructible", false)]
		private static void MakeDestructible(MenuCommand menuCommand)
		{
			AddSingleComponent<D2dDestructible>(GetGameObject(menuCommand));
		}
		
		[UnityEditor.MenuItem("CONTEXT/SpriteRenderer/" + ContextMenuPrefix + "Make Destructible (Preset: Dynamic Splittable)", true)]
		private static bool MakeDestructibleDynSplValidate(MenuCommand menuCommand)
		{
			return AddSingleComponentValidate<D2dDestructible>(GetGameObject(menuCommand));
		}
		
		[UnityEditor.MenuItem("CONTEXT/SpriteRenderer/" + ContextMenuPrefix + "Make Destructible (Preset: Dynamic Splittable)", false)]
		private static void MakeDestructibleDynSpl(MenuCommand menuCommand)
		{
			var gameObject = GetGameObject(menuCommand);
			
			AddSingleComponent<D2dDestructible>(gameObject, d => d.AutoSplit = true);
			AddSingleComponent<D2dPolygonCollider>(gameObject);
			AddSingleComponent<Rigidbody2D>(gameObject);
			AddSingleComponent<D2dRetainVelocity>(gameObject);
			AddSingleComponent<D2dCalculateMass>(gameObject);
			AddSingleComponent<D2dDestroyer>(gameObject, d => d.enabled = false);
			AddSingleComponent<D2dRequirements>(gameObject, r =>
				{
					var eventHandler   = new D2dEvent();
					var enableMethod   = typeof(D2dDestroyer).GetProperty("enabled").GetSetMethod();
					var enableDelegate = (UnityAction<bool>)System.Delegate.CreateDelegate(typeof(UnityAction<bool>), r.GetComponent<D2dDestroyer>(), enableMethod);
					
					UnityEditor.Events.UnityEventTools.AddBoolPersistentListener(eventHandler, enableDelegate, true);
					
					r.UseAlphaCountMax = true;
					r.AlphaCountMax    = 10;
					r.OnRequirementMet = eventHandler;
				});
		}
		
		[UnityEditor.MenuItem("CONTEXT/SpriteRenderer/" + ContextMenuPrefix + "Make Destructible (Preset: Dynamic Solid)", true)]
		private static bool MakeDestructibleDynSolValidate(MenuCommand menuCommand)
		{
			return AddSingleComponentValidate<D2dDestructible>(GetGameObject(menuCommand));
		}
		
		[UnityEditor.MenuItem("CONTEXT/SpriteRenderer/" + ContextMenuPrefix + "Make Destructible (Preset: Dynamic Solid)", false)]
		private static void MakeDestructibleDynSol(MenuCommand menuCommand)
		{
			var gameObject = GetGameObject(menuCommand);
			
			AddSingleComponent<D2dDestructible>(gameObject);
			AddSingleComponent<D2dPolygonCollider>(gameObject);
			AddSingleComponent<Rigidbody2D>(gameObject);
		}
		
		[UnityEditor.MenuItem("CONTEXT/SpriteRenderer/" + ContextMenuPrefix + "Make Destructible (Preset: Static)", true)]
		private static bool MakeDestructibleStaValidate(MenuCommand menuCommand)
		{
			return AddSingleComponentValidate<D2dDestructible>(GetGameObject(menuCommand));
		}
		
		[UnityEditor.MenuItem("CONTEXT/SpriteRenderer/" + ContextMenuPrefix + "Make Destructible (Preset: Static)", false)]
		private static void MakeDestructibleSta(MenuCommand menuCommand)
		{
			var gameObject = GetGameObject(menuCommand);
			
			AddSingleComponent<D2dDestructible>(gameObject);
			AddSingleComponent<D2dEdgeCollider>(gameObject);
		}
		
		[UnityEditor.MenuItem("CONTEXT/D2dDestructible/Add Fixture")]
		private static void AddFixture(MenuCommand menuCommand)
		{
			var gameObject = GetGameObject(menuCommand);
			var child      = new GameObject("Fixture");
			
			child.transform.SetParent(gameObject.transform, false);
			
			child.AddComponent<D2dFixture>();
			
			Selection.activeGameObject = child;
			
			EditorGUIUtility.PingObject(child);
		}
		
		private static bool AddSingleComponentValidate<T>(GameObject gameObject)
			where T : Component
		{
			if (gameObject != null)
			{
				return gameObject.GetComponent<T>() == null;
			}
			
			return false;
		}
		
		private static void AddSingleComponent<T>(GameObject gameObject, System.Action<T> action = null)
			where T : Component
		{
			if (gameObject != null)
			{
				if (gameObject.GetComponent<T>() == null)
				{
					var component = Undo.AddComponent<T>(gameObject);
					
					if (action != null)
					{
						action(component);
					}
				}
			}
		}
		
		private static GameObject GetGameObject(MenuCommand menuCommand)
		{
			if (menuCommand != null)
			{
				var component = menuCommand.context as Component;
				
				if (component != null)
				{
					return component.gameObject;
				}
			}
			
			return null;
		}
	}
}