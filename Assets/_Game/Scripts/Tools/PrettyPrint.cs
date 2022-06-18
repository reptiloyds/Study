using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using _Game.Scripts.ScriptableObjects;

namespace _Game.Scripts.Tools
{
	public static class PrettyPrint
	{
		private static Localization _localization;
		private static string[] _localizedPrefixes = new string[0];

		public static void Init(Localization localization)
		{
			_localization = localization;
			UpdateLocalization();
		}

		private static void UpdateLocalization()
		{
			var prefixes = new List<string>();

			var exp = 0;
			while (true)
			{
				exp += 3;
				var token = $"exp{exp}";
				var localized = _localization.Get(token);
				if (token == localized) break;
				
				prefixes.Add(localized);
			}

			_localizedPrefixes = prefixes.ToArray();
		}

		public static string ToLocalized(this string token)
		{
			return _localization.Get(token);
		}
		
		public static string ToFormattedString(this float value, string format = "0.##")
		{
			var (v, exp) = GetMantissaAndExp(value);
			var strValue = v.ToString(format, CultureInfo.InvariantCulture);

			if (exp == 0) return strValue;

			var hasDot = strValue.Contains(".");
			if (strValue.Length < 3 && !hasDot)
			{
				strValue += '.';
				hasDot = true;
			}

			if (hasDot)
			{
				while (strValue.Length < 4) strValue += '0';
			}

			var suffix = _localizedPrefixes.Length < exp ? $"e{exp * 3}" : _localizedPrefixes[exp - 1];

			return $"{strValue}{suffix}";
		}

		public static string ToFormattedRoundedString(this double value, string format = "0.##")
		{
			var (v, exp) = GetMantissaAndExp(value);
			var strValue = ((int)v).ToString();

			if (exp == 0) return strValue;

			var suffix = _localizedPrefixes.Length < exp ? $"e{exp * 3}" : _localizedPrefixes[exp - 1];

			return $"{strValue}{suffix}";
		}
		
		public static (double, int) GetMantissaAndExp(double value)
		{
			var exp = 0;
			double expValue = 1000;

			if (value < expValue) return (value, 0);

			while (value >= expValue)
			{
				exp++;
				expValue *= 1000;
			}

			expValue /= 1000;

			return (value / expValue, exp);
		}
		
		#region Time

		public struct TimeInfo
		{
			public int Days { get; private set; }
			public int Hours { get; private set; }
			public int Minutes { get; private set; }
			public int Seconds { get; private set;}

			public TimeInfo(float seconds)
			{
				Seconds = (int)Math.Ceiling(seconds);

				Minutes = Seconds / 60;
				Seconds -= Minutes * 60;

				Hours = Minutes / 60;
				Minutes -= Hours * 60;

				Days = Hours / 24;
				Hours -= Days * 24;
			}
		}
		
		public static TimeInfo ToTimeInfo(this float timeInSeconds)
		{
			return new TimeInfo(timeInSeconds);
		}

		public static string ToTimeStr(this float timeInSeconds, 
			int maxValuesCount = 2, 
			bool printZeros = false,
			bool printSuffix = false)
		{
			var info = new TimeInfo(timeInSeconds);
			return ToTimeStr(info, maxValuesCount, printZeros);
		}

		public static string ToTimeStr(this TimeInfo time, 
			int maxValuesCount = 2, 
			bool printZeros = false,
			bool printSuffix = false)
		{
			var space = printSuffix ? " " : ":";
			var timeBuilder = new StringBuilder();
			var valuesCount = 0;

			if (time.Days != 0)
			{
				timeBuilder.Append(printSuffix ? $"{time.Days}{"DD".ToLocalized()}" : time.Days);
				if (++valuesCount == maxValuesCount) return timeBuilder.ToString();
			}

			if (time.Hours != 0 || (printZeros && time.Days > 0))
			{
				if (timeBuilder.Length > 0) timeBuilder.Append(space);
				timeBuilder.Append(printSuffix ? $"{time.Hours}{"HH".ToLocalized()}" : time.Hours);
				if (++valuesCount == maxValuesCount) return timeBuilder.ToString();
			}

			if (time.Minutes != 0 || (printZeros && (time.Days + time.Hours) > 0))
			{
				if (timeBuilder.Length > 0) timeBuilder.Append(space);
				timeBuilder.Append(printSuffix ? $"{time.Minutes}{"MM".ToLocalized()}" : time.Minutes);
				if (++valuesCount == maxValuesCount) return timeBuilder.ToString();
			}

			if (time.Seconds != 0 || (printZeros && (time.Days + time.Hours + time.Minutes) > 0) || timeBuilder.Length == 0)
			{
				if (timeBuilder.Length > 0) timeBuilder.Append(space);
				timeBuilder.Append(printSuffix ? $"{time.Seconds}{"SS".ToLocalized()}" : time.Seconds);
			}

			return timeBuilder.ToString();
		}

		#endregion
	}
}