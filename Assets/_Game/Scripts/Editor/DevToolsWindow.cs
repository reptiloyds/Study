using System.Linq;
using _Game.Scripts.Balance;
using _Game.Scripts.ScriptableObjects;
using _Game.Scripts.Tools;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;
using static _Game.Scripts.Tools.EditorUITools;

namespace _Game.Scripts.Editor
{
	public class DevToolsWindow : EditorWindow
	{
		[MenuItem("_Game/Dev tools", false, -10)]
		static void Init()
		{
			var window = (DevToolsWindow)GetWindow(typeof(DevToolsWindow));
			window.Show();
			window.titleContent.text = "Dev tools";
		}

		private void OnGUI()
		{
			if (Button("Run test")) TestTools.RunMagicTest();
			
			DrawGoogleParsing();
			Space();
			DrawOpenSection();
			DrawSaves();
			DrawSkips();
		}

		private void DrawSaves()
		{
			BeginHorizontal();
			
			if (Application.isPlaying)
			{
				if (Button("Save"))
				{
					
				}
			}
			else
			{
				if (Button("Delete"))
				{
					File.Delete(Application.dataPath + "/data.json");
					PlayerPrefs.DeleteAll();
					PlayerPrefs.Save();
				}

				if (Button("Restore"))
				{
				}
			}
			
			EndHorizontal();
			Space();
		}

		private void DrawSkips()
		{
			// if (Application.isPlaying || !GameSave.Exists()) return;
			//
			// BeginHorizontal();
			// Label("Skip:");
			// if (Button("1m")) TestTools.SkipTime(1);
			// if (Button("1h")) TestTools.SkipTime(60);
			// if (Button("1d")) TestTools.SkipTime(60 * 24);
			// EndHorizontal();
		}

		private void Select(Object obj)
		{
			Selection.activeObject = obj;
		}

		private void DrawGoogleParsing()
		{
			if (Application.isPlaying) return;

			var parser = GameBalanceConfigs.Instance;
			BeginHorizontal();
			Label("Parse google:");
			if (Button("All")) parser.ParseAllTables();
			foreach (var table in parser.TableInfos.Where(table => Button(table.Name)))
			{
				parser.ParseTable(table);
			}

			EndHorizontal();
		}

		private void DrawOpenSection()
		{
			BeginHorizontal();
			Label("Open:");
			if (Button("Balance")) Select(GameBalanceConfigs.Instance);
			if (Button("Prefabs")) Select(Prefabs.Instance);
			if (Button("Resources")) Select(GameResources.Instance);
			if (Button("Loc")) Select(Localization.Instance);
			EndHorizontal();
			Space();
		}
	}
}