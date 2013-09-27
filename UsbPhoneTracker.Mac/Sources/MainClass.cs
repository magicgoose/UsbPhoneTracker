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

		static void HandleUsbChanged(DeviceIds device, Boolean connected)
		{
			if (connected)
			{
				var allDeviceInfo = DeviceInfoFinder.GetAllDevicesInfo();
				var deviceInfo = allDeviceInfo.FirstOrDefault(DeviceInfoFinder.IDMatch(device.ProductId, device.VendorId));
				if (deviceInfo == null)
				{
					Console.WriteLine("Can't find connected device!");
					return;
				}
				var serialNumber = deviceInfo["Serial Number"];
				Console.WriteLine("Vendor ID: {0}\nDevice ID: {1}\nSerial Number: {2}\n",
					device.VendorId,
					device.ProductId,
					serialNumber);
			}
			else
			{
				Console.WriteLine("A device has been disconnected\n");
			}
		}
	}
}

