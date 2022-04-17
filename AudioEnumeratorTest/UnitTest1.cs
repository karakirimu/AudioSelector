using AudioTools;
using NativeCoreAudio;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace AudioToolsTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void EnumDevices()
        {
            IReadOnlyCollection<MultiMediaDevice> devices
                = Enumeration.ListActiveRenderDevices();

            foreach (MultiMediaDevice device in devices)
            {
                Console.WriteLine($"Name: {device.DeviceName}, ID: {device.Id}");

                foreach (var connector in device.Connectors)
                {
                    Console.WriteLine($"Connected: {connector.Connected}," +
                        $" DataFlow: {connector.Flow}," +
                        $" ConnectionType: {connector.Type}");
                }
            }
        }

        [Test]
        public void GetDefaultEndpoint()
        {
            Console.WriteLine($"Default Endpoint: " +
                $"{Enumeration.GetDefaultDeviceEndpointId(ComInterfaces.ERole.eMultimedia)}");
            Assert.Pass();
        }

        [Test]
        public void SetDefaultEndpoint()
        {
            string speakerId = "{0.0.0.00000000}.{<uuid of device>}";

            using SafeIPolicyConfig config = new();

            try
            {
                config.SetDefaultEndpoint(speakerId, ComInterfaces.ERole.eConsole);
                config.SetDefaultEndpoint(speakerId, ComInterfaces.ERole.eMultimedia);
                config.SetDefaultEndpoint(speakerId, ComInterfaces.ERole.eCommunications);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Assert.Fail();
            }

            Assert.Pass();
        }

        [Test]
        public void SetDefaultEndpointForPolicy()
        {
            //string speakerId = "{0.0.0.00000000}.{ce30c242-d7a0-4ce4-ad9b-0b68804e069a}";

            //using SafeIPolicyConfig config = new();

            //try
            //{
            //    config.SetDefaultEndpointForPolicy(speakerId, ComInterfaces.EDataFlow.eAll);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //    Assert.Fail();
            //}

            Assert.Pass();
        }

        [Test]
        public void NotifiCationEventEnable()
        {
            NotificationEvent clientEvent = new();
            clientEvent.EnableNotification();
            clientEvent.DisableNotification();

            Assert.Pass();
        }
    }
}