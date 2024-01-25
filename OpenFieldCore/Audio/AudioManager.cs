using OFC.Utility;
using OpenTK.Audio.OpenAL;

using System;
using System.Collections.Generic;

namespace OFC.Audio
{
    public static class AudioManager
    {
        // CPU Data
        static bool isInitialized = false;

        // SPU Data
        static IEnumerable<string> spuOutputDevices;
        static IEnumerable<string> spuInputDevices;
        static ALDevice  spuDevice;
        static ALContext spuContext;

        public static void Initialize()
        {
            if (isInitialized)
                return;

            //
            // Construct OpenAL Devices
            //

            // Find Output Devices...
            //spuOutputDevices = ALC.GetStringList(GetEnumerationStringList.DeviceSpecifier);
            //foreach(string odevice in spuOutputDevices)
            //    Log.Info($"Audio Output Device [name = {odevice}]");

            // Find Input Devices...
            //spuInputDevices = ALC.GetStringList(GetEnumerationStringList.CaptureDeviceSpecifier);
            //foreach (string idevice in spuInputDevices)
            //    Log.Info($"Audio Input Device [name = {idevice}, strlen = {idevice.Length}]");

            // Ignore whatever we found and just use the default device :)
            spuDevice = ALC.OpenDevice(null);
            if (spuDevice.Handle == IntPtr.Zero)
            {
                Log.Error("Couldn't create OpenAL device!");
                goto FailInit;
            }

            // Create context, with the default attributes
            spuContext = ALC.CreateContext(spuDevice, (int[])null);
            if(spuContext.Handle == IntPtr.Zero)
            {
                Log.Error("Couldn't create OpenAL context!");
                goto FailInit;
            }

            // Make context current.
            if(!ALC.MakeContextCurrent(spuContext))
            {
                Log.Error("Couldn't set OpenAL context as current!");
                goto FailInit;
            }
            
            InterrogateAL();

            isInitialized = true;
            return;

        FailInit:
            throw new Exception("Failed to initialize AudioManager!");
        }

        private static void InterrogateAL()
        {
            Console.WriteLine("OpenAL Specification:");
            Console.WriteLine($"\tVendor     = {AL.Get(ALGetString.Vendor)}");
            Console.WriteLine($"\tVersion    = {AL.Get(ALGetString.Version)}");
            Console.WriteLine($"\tRenderer   = {AL.Get(ALGetString.Renderer)}");
            Console.WriteLine($"\tMax Mono Sources   = {ALC.GetContextAttributes(spuDevice).MonoSources}");
            Console.WriteLine($"\tMax Stereo Sources = {ALC.GetContextAttributes(spuDevice).StereoSources}");
            Console.WriteLine($"\tMax Frequency      = {ALC.GetContextAttributes(spuDevice).Frequency}");
            Console.WriteLine($"\tExtensions = {{\n\t\t{AL.Get(ALGetString.Extensions).Replace(" ", ",\n\t\t") + ",\n\t\t" + ALC.GetString(spuDevice, AlcGetString.Extensions).Replace(" ", ",\n\t\t")}\n\t}}");

            Console.WriteLine($"\tOutput Devices = {{\n\t\t{string.Join(",\n\t\t", ALC.GetString(spuDevice, AlcGetStringList.AllDevicesSpecifier))}\n\t}}");
        }
    }
}
