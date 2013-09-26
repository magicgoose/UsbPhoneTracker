using System;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using LibUsbDotNet.Info;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace UsbPhoneTracker.Common
{
	public class UsbNotifier
	{
		static LibUsbDotNet.DeviceNotify.IDeviceNotifier _notifier;

		public static event Action<UsbChange> UsbChanged;

		public static void Start()
		{
			_notifier = LibUsbDotNet.DeviceNotify.DeviceNotifier.OpenDeviceNotifier();
			_notifier.OnDeviceNotify += HandleDeviceNotify;
		}

		public static void Stop()
		{
			UsbDevice.Exit();
			_notifier = null;
			UsbChanged = null;
		}

		static void HandleDeviceNotify (object sender, LibUsbDotNet.DeviceNotify.DeviceNotifyEventArgs e)
		{
			var device = e.Device;
			var changeInfo = new UsbChange(
				device.IdProduct,
				device.IdVendor,
				e.EventType == LibUsbDotNet.DeviceNotify.EventType.DeviceArrival);
			ProcessUsbChange(changeInfo);
		}

		static void ProcessUsbChange(UsbChange change)
		{
			if (UsbChanged != null)
				UsbChanged(change);
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
