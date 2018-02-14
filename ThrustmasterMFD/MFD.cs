using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HidSharp;

namespace ThrustmasterMFD
{
    public class MFD
    {
        private HidDevice _device;
        const int THRUSTMASTER_VENDOR_ID = 0x044F;
        const int MFD1_PRODUCT_ID = 0xB351;
        const int MFD2_PRODUCT_ID = 0xB352;

        MFD(int id)
        {
            this.Id = id;
            var loader = new HidDeviceLoader();
            if (id == 1)
            {
                _device = loader.GetDevices(THRUSTMASTER_VENDOR_ID, MFD1_PRODUCT_ID).First();
                if (_device == null)
                {
                    throw new Exception("MFD1 Device not found");
                }

            }
            else
            if (id == 2)
            {
                _device = loader.GetDevices(THRUSTMASTER_VENDOR_ID, MFD2_PRODUCT_ID).First();
                if (_device == null)
                {
                    throw new Exception("MFD2 Device not found");
                }
            }
            else
            {
                throw new InvalidOperationException("Invalid MFD ID selected");
            }
        }

        public int Id
        {
            get; set;
        }

        public byte GetLEDStatus()
        {
            using (var stream = _device.Open())
            {
                var hidbuffer = new byte[_device.MaxFeatureReportLength];
                hidbuffer[0] = 0x00;
                stream.GetFeature(hidbuffer);
                return hidbuffer[1];
            }
        }

        public void SetLEDStatus(bool left, bool right)
        {
            using (var stream = _device.Open())
            {
                byte mask = 0;
                if (right)
                    mask += 1;
                if (left)
                    mask += 2;
                var hidbuffer = new byte[] { 0x00, 0x00, mask };
                stream.SetFeature(hidbuffer);
            }
        }

        public byte GetBrightness()
        {
            using (var stream = _device.Open())
            {
                var hidbuffer = new byte[_device.MaxFeatureReportLength];
                hidbuffer[0] = 0x00;
                stream.GetFeature(hidbuffer);
                return hidbuffer[2];
            }
        }

        public void SetBrightness(byte brightness)
        {
            using (var stream = _device.Open())
            {
                var hidbuffer = new byte[] { 0x00, 0x01, brightness };
                stream.SetFeature(hidbuffer);
            }
        }

    }
}
