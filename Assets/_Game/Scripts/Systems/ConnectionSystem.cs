using System;
using System.Collections;
using System.Globalization;
using _Game.Scripts.Interfaces;
using _Game.Scripts.ScriptableObjects;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace _Game.Scripts.Systems
{
	/// <summary>
	/// Класс для проверки интернет соединения при запуске и во время игры
	/// </summary>
	public class ConnectionSystem : MonoBehaviour, ITickableSystem
	{
		public Action<bool> Connected;
		
		public DateTime ServerTime { get; private set; }
		
		private ProjectSettings _projectSettings;

		[Inject]
		private void Construct(ProjectSettings settings)
		{
			_projectSettings = settings;
		}
	
		private float _checkInternetTimer;

		public void RunCheckConnection(bool setTime = true)
		{
			if (_projectSettings == null)
			{
				Connected?.Invoke(false);
				return;
			}
			
			StartCoroutine(GetRequest(setTime));
		}

		private IEnumerator GetRequest(bool setTime)
		{
			using var webRequest = UnityWebRequest.Get(_projectSettings.Host);
			yield return webRequest.SendWebRequest();
			
			if(!webRequest.isDone)
			{
				Connected?.Invoke(false);
			}
			else 
			{
				if (setTime)
				{
					var results = webRequest.GetResponseHeader("date");
					ServerTime = DateTime.ParseExact(results,
						"ddd, dd MMM yyyy HH:mm:ss 'GMT'",
						CultureInfo.InvariantCulture.DateTimeFormat,
						DateTimeStyles.AssumeUniversal);

				}
				
				Connected?.Invoke(true);
			}
		}

		public void Tick(float deltaTime)
		{
#if UNITY_EDITOR
			ServerTime = ServerTime.AddSeconds(deltaTime / Time.timeScale);
#else
			ServerTime = ServerTime.AddSeconds(deltaTime);
#endif
		}
	}
}