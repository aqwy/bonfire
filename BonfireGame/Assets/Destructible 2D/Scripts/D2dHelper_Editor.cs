#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Destructible2D
{
	public static partial class D2dHelper
	{
		private static GUIStyle none;

		private static GUIStyle error;

		private static GUIStyle noError;

		public static GUIStyle None
		{
			get
			{
				if (none == null)
				{
					none = new GUIStyle();
				}

				return none;
			}
		}

		public static GUIStyle Error
		{
			get
			{
				if (error == null)
				{
					error                   = new GUIStyle();
					error.border            = new RectOffset(3, 3, 3, 3);
					error.normal            = new GUIStyleState();
					error.normal.background = CreateTempTexture(12, 12, "iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAALElEQVQIHWP4z8CgC8SHgfg/lNZlQBIACYIlGEEMBjTABOQfQRM7AlKGYSYAoOwcvDRV9/MAAAAASUVORK5CYII=");
				}

				return error;
			}
		}

		public static GUIStyle NoError
		{
			get
			{
				if (noError == null)
				{
					noError        = new GUIStyle();
					noError.border = new RectOffset(3, 3, 3, 3);
					noError.normal = new GUIStyleState();
				}

				return noError;
			}
		}

		public static void MakeTextureReadable(Texture2D texture)
		{
			if (texture != null)
			{
				var importer = GetAssetImporter<UnityEditor.TextureImporter>(texture);

				if (importer != null && importer.isReadable == false)
				{
					importer.isReadable = true;

					ReimportAsset(importer.assetPath);
				}
			}
		}

		public static T GetAssetImporter<T>(Object asset)
			where T : AssetImporter
		{
			return GetAssetImporter<T>((AssetDatabase.GetAssetPath(asset)));
		}

		public static T GetAssetImporter<T>(string path)
			where T : AssetImporter
		{
			return (T)AssetImporter.GetAtPath(path);
		}

		public static void ReimportAsset(Object asset)
		{
			ReimportAsset(AssetDatabase.GetAssetPath(asset));
		}

		public static void ReimportAsset(string path)
		{
			AssetDatabase.ImportAsset(path);
		}

		public static Rect Reserve(float height = 16.0f, bool indent = false)
		{
			var rect = default(Rect);

			EditorGUILayout.BeginVertical(NoError);
			{
				rect = EditorGUILayout.BeginVertical();
				{
					EditorGUILayout.LabelField(string.Empty, GUILayout.Height(height), GUILayout.ExpandWidth(true), GUILayout.MinWidth(0.0f));
				}
				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndVertical();

			if (indent == true)
			{
				rect = EditorGUI.IndentedRect(rect);
			}

			return rect;
		}

		public static Texture2D CreateTempTexture(int width, int height, string encoded)
		{
			var texture = new Texture2D(width, height, TextureFormat.ARGB32, false);

			texture.hideFlags = HideFlags.HideAndDontSave;
			texture.LoadImage(System.Convert.FromBase64String(encoded));
			texture.Apply();

			return texture;
		}

		public static void SetDirty<T>(T t)
			where T : Object
		{
			if (t != null)
			{
				EditorUtility.SetDirty(t);
			}
		}

		public static void SetDirty<T>(T[] ts)
			where T : Object
		{
			foreach (var t in ts)
			{
				SetDirty(t);
			}
		}

		public static void SetDirty(Object target)
		{
			UnityEditor.EditorUtility.SetDirty(target);

#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			UnityEditor.EditorApplication.MarkSceneDirty();
#else
			UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
#endif
		}
	}
}
#endif