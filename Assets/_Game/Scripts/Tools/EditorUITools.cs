using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace _Game.Scripts.Tools
{
	public static class EditorUITools
	{
#if UNITY_EDITOR
		private static readonly Dictionary<EditorWindow, Vector2> _scrollPositions = new Dictionary<EditorWindow, Vector2>();	
#endif
		public static bool Button(string text, int width = 0, int height = 0)
		{
			if (width == 0 && height == 0) return GUILayout.Button(text);
			var options = new List<GUILayoutOption>();

			if (width > 0) options.Add(GUILayout.Width(width));
			if (height > 0) options.Add(GUILayout.Height(height));

			return GUILayout.Button(text, options.ToArray());
		}

		public static void Label(string text, int width = 0)
		{
			if (width == 0) GUILayout.Label(text);
			else GUILayout.Label(text, GUILayout.Width(width));
		}

		public static void BeginHorizontal()
		{
			GUILayout.BeginHorizontal();
		}

		public static void EndHorizontal(bool flexible = false)
		{
			if (flexible) GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		public static void Space(int space = 10)
		{
			GUILayout.Space(space);
		}

		public static void FlexibleSpace()
		{
			GUILayout.FlexibleSpace();
		}

		public static string TextField(string text, int width)
		{
			if (width == 0) return GUILayout.TextField(text);
			return GUILayout.TextField(text, GUILayout.Width(width));
		}

#if UNITY_EDITOR
		public static void BeginScrollView(EditorWindow window)
		{
			if (!_scrollPositions.ContainsKey(window)) _scrollPositions.Add(window, Vector2.zero);
			_scrollPositions[window] = EditorGUILayout.BeginScrollView(_scrollPositions[window]);
		}

		public static void EndScrollView()
		{
			EditorGUILayout.EndScrollView();
		}
#endif
	}
}