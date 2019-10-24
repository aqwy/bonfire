using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dGui))]
	public class D2dGui_Editor : D2dEditor<D2dGui>
	{
		protected override void OnInspector()
		{
			DrawDefault("Header");
			DrawDefault("Footer");
		}
	}
}
#endif

namespace Destructible2D
{
	// This component shows a simple GUI with FPS
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "GUI")]
	public class D2dGui : MonoBehaviour
	{
		[Tooltip("The text that appears at the top of the screen")]
		[Multiline]
		public string Header;
		
		[Tooltip("The text that appears at the bottom of the screen")]
		[Multiline]
		public string Footer;
		
		// Elapsed seconds from last second
		private float counter;
		
		// Elapsed frames from last second
		private int frames;
		
		// Previous fps
		private float fps;
		
		// Cached GUI styles
		private static GUIStyle whiteStyle;
		private static GUIStyle blackStyle;
		
		protected virtual void Update()
		{
			counter += Time.deltaTime;
			frames  += 1;
			
			if (counter >= 1.0f)
			{
				fps = (float)frames / counter;
				
				counter = 0.0f;
				frames  = 0;
			}
		}
		
		protected virtual void OnGUI()
		{
			var r1 = new Rect(5, 50 + 55 * 0, 100, 50);
			var r2 = new Rect(5, 50 + 55 * 1, 100, 50);
			var r3 = new Rect(5, 50 + 55 * 2, 100, 50);
			
			if (GUI.Button(r1, "Reload") == true)
			{
				LoadLevel(GetCurrentLevel());
			}
			
			if (GUI.Button(r2, "Prev") == true)
			{
				var index = GetCurrentLevel() - 1;

				if (index < 0)
				{
					index = GetLevelCount() - 1;
				}

				LoadLevel(index);
			}
			
			if (GUI.Button(r3, "Next") == true)
			{
				var index = GetCurrentLevel() + 1;

				if (index >= GetLevelCount())
				{
					index = 0;
				}

				LoadLevel(index);
			}
			
			// Draw FPS?
			if (fps > 0.0f)
			{
				DrawText("FPS: " + fps.ToString("0"), TextAnchor.UpperLeft);
			}
			
			// Draw header?
			if (string.IsNullOrEmpty(Header) == false)
			{
				DrawText(Header, TextAnchor.UpperCenter, 150);
			}
			
			// Draw footer?
			if (string.IsNullOrEmpty(Footer) == false)
			{
				DrawText(Footer, TextAnchor.LowerCenter, 150);
			}
		}

#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
		private static int GetCurrentLevel()
		{
			return Application.loadedLevel;
		}

		private static int GetLevelCount()
		{
			return Application.levelCount;
		}

		private static void LoadLevel(int index)
		{
			Application.LoadLevel(index);
		}
#else
		private static int GetCurrentLevel()
		{
			return UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
		}

		private static int GetLevelCount()
		{
			return UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
		}

		private static void LoadLevel(int index)
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene(index);
		}
#endif
		
		private static void DrawText(string text, TextAnchor anchor, int offsetX = 15, int offsetY = 15)
		{
			if (string.IsNullOrEmpty(text) == false)
			{
				if (whiteStyle == null || blackStyle == null)
				{
					whiteStyle = new GUIStyle();
					whiteStyle.fontSize  = 20;
					whiteStyle.fontStyle = FontStyle.Bold;
					whiteStyle.wordWrap  = true;
					whiteStyle.normal    = new GUIStyleState();
					whiteStyle.normal.textColor = Color.white;
					
					blackStyle = new GUIStyle();
					blackStyle.fontSize  = 20;
					blackStyle.fontStyle = FontStyle.Bold;
					blackStyle.wordWrap  = true;
					blackStyle.normal    = new GUIStyleState();
					blackStyle.normal.textColor = Color.black;
				}
				
				whiteStyle.alignment = anchor;
				blackStyle.alignment = anchor;
				
				var sw   = (float)Screen.width;
				var sh   = (float)Screen.height;
				var rect = new Rect(0, 0, sw, sh);
				
				rect.xMin += offsetX;
				rect.xMax -= offsetX;
				rect.yMin += offsetY;
				rect.yMax -= offsetY;
				
				rect.x += 1;
				GUI.Label(rect, text, blackStyle);
				
				rect.x -= 2;
				GUI.Label(rect, text, blackStyle);
				
				rect.x += 1;
				rect.y += 1;
				GUI.Label(rect, text, blackStyle);
				
				rect.y -= 2;
				GUI.Label(rect, text, blackStyle);
				
				rect.y += 1;
				GUI.Label(rect, text, whiteStyle);
			}
		}
	}
}