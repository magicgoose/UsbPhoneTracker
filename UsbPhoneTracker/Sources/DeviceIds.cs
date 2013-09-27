using System;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using LibUsbDotNet.Info;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace UsbPhoneTracker.Common
{
	public struct DeviceIds
	{
		public readonly Int32 ProductId;
		public readonly Int32 VendorId;

		public DeviceIds(int productId, int vendorId)
		{
			this.ProductId = productId;
			this.VendorId = vendorId;
		}

		public override String ToString()
		{
			return String.Format("[UsbChange: ProductId={0}, VendorId={1}]", ProductId, VendorId);
		}
	}
}
