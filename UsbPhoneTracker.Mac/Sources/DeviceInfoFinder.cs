using System;
using UsbPhoneTracker.Common;
using System.Collections.Generic;
using System.Linq;

namespace UsbPhoneTracker.Mac
{
	public static class DeviceInfoFinder
	{
		public static Tuple<DeviceIds, String> Extract(Dictionary<string, string> x)
		{
			try
			{
				var pid = Convert.ToInt32(x["Product ID"].Split(' ').First(), 16);
				var vid = Convert.ToInt32(x["Vendor ID"].Split(' ').First(), 16);
				var serial = x["Serial Number"];
				return Tuple.Create(new DeviceIds(pid, vid), serial);
			}
			catch
			{
				return null;
			}
		}

		public static Boolean IsIDMatch(Dictionary<string, string> x, DeviceIds ids)
		{
			var e = Extract(x);
			return e != null && e.Item1.Equals(ids);
		}

		public static Func<Dictionary<string, string>, Boolean> IDMatch(DeviceIds ids)
		{
			return d => IsIDMatch(d, ids);
		}

		public static IEnumerable<Dictionary<String, String>> GetAllDevicesInfo()
		{
			return ParseAll(
				GroupByConsecutive(
					IndentCount,
					GetLines(
						UsbNotifier.RunCommand(
							"system_profiler",
							"SPUSBDataType"))));
		}

		static IEnumerable<Dictionary<String, String>> ParseAll(IEnumerable<List<String>> groups)
		{
			try
			{
				return groups.Where(x => x.Count > 1)
					.Select(ParseGroup)
					.Where(x => x != null);
			}
			catch
			{
				return Enumerable.Empty<Dictionary<String, String>>();
			}
		}

		static Dictionary<String, String> ParseGroup(IEnumerable<String> group)
		{
			try
			{
				var result = new Dictionary<String, String>();
				foreach (var line in group)
				{
					var kv = line.Split(':').Select(x => x.Trim()).ToArray();
					var key = kv[0];
					var value = kv[1];
					result.Add(key, value);
				}
				return result;
			}
			catch
			{
				return null;
			}
		}

		static IEnumerable<String> GetLines(String x)
		{
			return x.Split(Environment.NewLine.ToCharArray())
				.Select(_ => _.TrimEnd())
					.Where(_ => !String.IsNullOrWhiteSpace(_));
		}

		static Int32 IndentCount(String s)
		{
			return s.TakeWhile(x => x == ' ').Count();
		}

		// src must not be empty
		static IEnumerable<List<T>> GroupByConsecutive<T, K>(Func<T, K> keyFun, IEnumerable<T> src)
		{
			var e = src.GetEnumerator();
			e.MoveNext();
			var current = e.Current;
			var currentKey = keyFun(current);
			var chunk = new List<T> { current };
			while (e.MoveNext())
			{
				var next = e.Current;
				var nextKey = keyFun(next);
				if (EqualityComparer<K>.Default.Equals(nextKey, currentKey))
				{
					chunk.Add(next);
				}
				else
				{
					yield return chunk;
					current = next;
					currentKey = nextKey;
					chunk = new List<T> { current };
				}
			}
			if (chunk.Count > 0)
				yield return chunk;
		}
	}
}

