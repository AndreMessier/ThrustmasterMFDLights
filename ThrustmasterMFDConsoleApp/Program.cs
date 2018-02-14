using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HidSharp;

namespace ThrustmasterMFDConsoleApp
{
    class Program
    {
        static int THRUSTMASTER_VENDOR_ID = 0x044F;
        static int MFD1_PRODUCT_ID = 0xB351;
        static int MFD2_PRODUCT_ID = 0xB352;

        static void Main(string[] args)
        {
            var loader = new HidDeviceLoader();
            //var devices = loader.GetDevices(THRUSTMASTER_VENDOR_ID, MFD_PRODUCT_ID);
            var devices = loader.GetDevices(THRUSTMASTER_VENDOR_ID);

            var deviceMFD1 = loader.GetDevices(THRUSTMASTER_VENDOR_ID, MFD1_PRODUCT_ID).First();
            var deviceMFD2 = loader.GetDevices(THRUSTMASTER_VENDOR_ID, MFD2_PRODUCT_ID).First();

            if (deviceMFD1 != null)
            {
                byte brightness = 0x09;
                byte ledStatus = 0;

                var device = deviceMFD1;

                SetBrightness(brightness, device);
                brightness = GetBrightness(device);

                //SetLEDStatus(device, false, true);    // left light off, right light on
                //SetLEDStatus(device, true, false);   // left light on, right light on
                //SetLEDStatus(device, true, true);   // left light on, right light on
                SetLEDStatus(device, false, false);   // left light on, right light on
                ledStatus = GetLEDStatus(device);
            }

            Console.Write("Devices: ");
            Console.WriteLine(devices.Count());
            Console.WriteLine("Done!");
        }

        private static byte GetLEDStatus(HidDevice device)
        {
            using (var stream = device.Open())
            {
                // get status
                var hidbuffer = new byte[device.MaxFeatureReportLength];
                hidbuffer[0] = 0x00;
                stream.GetFeature(hidbuffer);
                Console.WriteLine("Read Feature Report: {0}", String.Join("-", hidbuffer));
                return hidbuffer[1];
            }
        }

        private static void SetLEDStatus(HidDevice device, bool left, bool right)
        {
            using (var stream = device.Open())
            {                
                byte mask = 0;
                if (right)
                    mask += 1;
                if (left)
                    mask += 2;
                var hidbuffer = new byte[] { 0x00, 0x00, mask };
                Console.WriteLine("Write Feature Report: {0}", String.Join("-", hidbuffer));
                stream.SetFeature(hidbuffer);
            }
        }

        private static byte GetBrightness(HidDevice device)
        {
            using (var stream = device.Open())
            {
                // get brightness
                var hidbuffer = new byte[device.MaxFeatureReportLength];
                hidbuffer[0] = 0x00;
                stream.GetFeature(hidbuffer);
                Console.WriteLine("Read Feature Report: {0}", String.Join("-", hidbuffer));
                return hidbuffer[2];
            }
        }

        private static void SetBrightness(byte brightness, HidDevice device)
        {
            using (var stream = device.Open())
            {
                // set brightness
                var hidbuffer = new byte[] { 0x00, 0x01, brightness };
                Console.WriteLine("Write Feature Report: {0}", String.Join("-", hidbuffer));
                stream.SetFeature(hidbuffer);
            }
        }
    }
}
