using System;
using UsbPhoneTracker.Common;
using System.Collections.Generic;
using System.Linq;

namespace UsbPhoneTracker.Mac
{
	public static class MainClass
	{
		static void Main(String[] args)
		{
			UsbNotifier.UsbChanged += HandleUsbChanged;
			UsbNotifier.Start();
			Console.ReadLine();
			UsbNotifier.Stop();
		}

		static void HandleUsbChanged(UsbChange change)
		{
			if (change.Connected)
			{
				var deviceInfo = GetDeviceInfo(change.ProductId, change.VendorId);
				if (deviceInfo == null)
				{
					Console.WriteLine("Can't find connected device!");
					return;
				}
				var serialNumber = deviceInfo["Serial Number"];
				Console.WriteLine("Vendor ID: {0}\nDevice ID: {1}\nSerial Number: {2}\n",
					change.VendorId,
					change.ProductId,
					serialNumber);
			}
			else
			{
				Console.WriteLine("A device has been disconnected\n");
			}
		}

		static Dictionary<String, String> GetDeviceInfo(Int32 Pid, Int32 Vid)
		{
			var ololo = UsbNotifier.RunCommand("system_profiler", "SPUSBDataType");
			var grouped = GroupByConsecutive(IndentCount, GetLines(ololo));
			var deviceGroup = FindDeviceGroup(Pid, Vid, grouped);
			return deviceGroup;
		}

		static Boolean CheckIds(Dictionary<string, string> x, int pid, int vid)
		{
			try
			{
				var _pid = Convert.ToInt32(x["Product ID"].Split(' ').First(), 16);
				var _vid = Convert.ToInt32(x["Vendor ID"].Split(' ').First(), 16);

				return _pid == pid && _vid == vid;
			}
			catch
			{
				return false;
			}
		}

		static Dictionary<String, String> FindDeviceGroup(Int32 Pid, Int32 Vid, IEnumerable<List<String>> groups)
		{
			try
			{
				return groups.Where(x => x.Count > 1)
					.Select(ParseGroup)
						.FirstOrDefault(x => CheckIds(x, Pid, Vid));
			}
			catch(Exception e)
			{
				return null;
			}
		}

		static Dictionary<String, String> ParseGroup(IEnumerable<String> group)
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

