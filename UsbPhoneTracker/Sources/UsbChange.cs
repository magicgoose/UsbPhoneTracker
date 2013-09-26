using System;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using LibUsbDotNet.Info;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace UsbPhoneTracker.Common
{
	public struct UsbChange
	{
		public readonly Int32 ProductId;
		public readonly Int32 VendorId;
		public readonly Boolean Connected;

		public UsbChange(int productId, int vendorId, bool connected)
		{
			this.ProductId = productId;
			this.VendorId = vendorId;
			this.Connected = connected;
		}

		public override String ToString()
		{
			return String.Format("[UsbChange: ProductId={0}, VendorId={1}, Connected={2}]", ProductId, VendorId, Connected);
		}
	}
}
