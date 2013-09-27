using System;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using LibUsbDotNet.Info;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace UsbPhoneTracker.Common
{
	public static class UsbNotifier
	{
		static LibUsbDotNet.DeviceNotify.IDeviceNotifier _notifier;

		public static event Action<DeviceIds, Boolean> UsbChanged;

		public static void Start()
		{
			_notifier = LibUsbDotNet.DeviceNotify.DeviceNotifier.OpenDeviceNotifier();
			_notifier.OnDeviceNotify += HandleDeviceNotify;
		}

		public static void Stop()
		{
			_notifier.OnDeviceNotify -= HandleDeviceNotify;
			_notifier.Enabled = false;
			_notifier = null;
			UsbDevice.Exit();
			UsbChanged = null;
		}

		static void HandleDeviceNotify (object sender, LibUsbDotNet.DeviceNotify.DeviceNotifyEventArgs e)
		{
			var device = e.Device;

			var connected = e.EventType == LibUsbDotNet.DeviceNotify.EventType.DeviceArrival;

			var changeInfo = new DeviceIds(
				device.IdProduct,
				device.IdVendor);
			ProcessUsbChange(changeInfo, connected);
		}

		static void ProcessUsbChange(DeviceIds device, Boolean connected)
		{
			if (UsbChanged != null)
				UsbChanged(device, connected);
		}

		public static String RunCommand(String command, String args)
		{
			// Start the child process.
			var p = new Process();
			// Redirect the output stream of the child process.
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.FileName = command;
			p.StartInfo.Arguments = args;
			p.Start();
			String output = p.StandardOutput.ReadToEnd();
			p.WaitForExit();
			return output;
		}
	}
}
