//Сделано по этому туториалу https://developers.google.com/sheets/api/quickstart/js
//Создать приложение можно здесь https://console.developers.google.com/projectcreate
//Информацию о приложениях можно посмотреть тут https://console.developers.google.com/home/dashboard

using System;
using System.Collections.Generic;
using System.IO;
using _Game.Scripts.Balance.BalanceParse;
using _Game.Scripts.ScriptableObjects;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.Balance
{
	[CreateAssetMenu(fileName = "GameBalanceConfigs", menuName = "_Game/GameBalanceConfigs", order = 1)]
	public class GameBalanceConfigs : ScriptableObjectInstaller
	{
		[SerializeField] private BalanceConfig _defaultBalance;
		[SerializeField] private TextAsset _credentials;
		[SerializeField] private List<TableInfo> _tableInfos;

		private static GameBalanceConfigs _instance;
		public static GameBalanceConfigs Instance
		{
			get
			{
				if (_instance == null) _instance = Resources.Load<GameBalanceConfigs>("Balance/" + nameof(GameBalanceConfigs));
				return _instance;
			}
		}
		
		public BalanceConfig DefaultBalance { get; private set; }
		public List<TableInfo> TableInfos => _tableInfos;
		
		public void SetDefaultBalance()
		{
			DefaultBalance = _defaultBalance;
		}
		
#if UNITY_EDITOR
		public void ParseAllTables()
		{
			foreach (var tableInfo in _tableInfos)
			{
				ParseTable(tableInfo);
			}
		}
		
		public void ParseTable(TableInfo tableInfo)
		{

			if (tableInfo == null)
			{
				Debug.LogError("Can't parse null table info");
				return;
			}

			var tableName = tableInfo.Name;
			var folderName = $"~{tableName}";
			var folderPath = $"{Application.dataPath}/{folderName}";
			
			GoogleCSVDownloader.DownloadSheetsAsCSV(tableInfo.SpreadSheetLink, folderPath, AssetDatabase.GetAssetPath(_credentials));
			
			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}

			ParseCSVToObject(tableInfo.ScriptableObject, folderPath, tableName);

			Directory.Delete(folderPath, true);

			Debug.Log($"{tableName} parsed");

		}
		
		private void ParseCSVToObject(ScriptableObject toObj, string folderPath, string workbookName)
		{
			var files = Directory.GetFiles(folderPath, "*.csv");
			var workbook = CSVReader.ParseCSV(workbookName, files, '\t');
			ValuesImporter.Import(toObj, workbook);
		}
#endif
		
		[Serializable]
		public class TableInfo
		{
			public string Name;
			public ScriptableObject ScriptableObject;
			public string SpreadSheetLink;
		}
	}
}