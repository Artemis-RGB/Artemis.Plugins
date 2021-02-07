using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Artemis.Plugins.PhilipsHue.RGB.NET.Hue;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Groups;
using Q42.HueApi.Streaming;
using Q42.HueApi.Streaming.Models;
using RGB.NET.Core;

namespace Artemis.Plugins.PhilipsHue.RGB.NET
{
    public class HueRGBDeviceProvider : IRGBDeviceProvider
    {
        private static HueRGBDeviceProvider _instance;

        public HueRGBDeviceProvider()
        {
            if (_instance != null) throw new InvalidOperationException($"There can be only one instance of type {nameof(HueRGBDeviceProvider)}");
            _instance = this;
        }

        public static HueRGBDeviceProvider Instance => _instance ?? new HueRGBDeviceProvider();
        public HueDeviceUpdateTrigger UpdateTrigger { get; } = new HueDeviceUpdateTrigger();
        public List<HueClientDefinition> ClientDefinitions { get; } = new List<HueClientDefinition>();

        public bool IsInitialized { get; private set; }
        public IEnumerable<IRGBDevice> Devices { get; private set; }
        public bool HasExclusiveAccess { get; } = false;

        public bool Initialize(RGBDeviceType loadFilter = RGBDeviceType.All, bool throwExceptions = false)
        {
            IsInitialized = false;

            try
            {
                UpdateTrigger.Stop();

                IList<IRGBDevice> devices = new List<IRGBDevice>();
                UpdateTrigger.ClientGroups = new Dictionary<StreamingHueClient, StreamingGroup>();

                foreach (HueClientDefinition clientDefinition in ClientDefinitions)
                {
                    // Create a temporary for this definition 
                    ILocalHueClient client = new LocalHueClient(clientDefinition.Ip);
                    client.Initialize(clientDefinition.AppKey);

                    // Get the entertainment groups, no point continuing without any entertainment groups
                    IReadOnlyList<Group> entertainmentGroups = client.GetEntertainmentGroups().GetAwaiter().GetResult();
                    if (!entertainmentGroups.Any())
                        continue;

                    // Get all lights once, all devices can use this list to identify themselves
                    List<Light> lights = client.GetLightsAsync().GetAwaiter().GetResult().ToList();

                    foreach (Group entertainmentGroup in entertainmentGroups.OrderBy(g => int.Parse(g.Id)))
                    {
                        StreamingHueClient streamingClient = new StreamingHueClient(clientDefinition.Ip, clientDefinition.AppKey, clientDefinition.ClientKey);
                        StreamingGroup streamingGroup = new StreamingGroup(entertainmentGroup.Locations);
                        streamingClient.Connect(entertainmentGroup.Id).GetAwaiter().GetResult();

                        UpdateTrigger.ClientGroups.Add(streamingClient, streamingGroup);
                        foreach (string lightId in entertainmentGroup.Lights.OrderBy(int.Parse))
                        {
                            HueDeviceInfo deviceInfo = new HueDeviceInfo(entertainmentGroup, lightId, lights);
                            HueDevice device = new HueDevice(deviceInfo);
                            device.Initialize(new HueUpdateQueue(UpdateTrigger, lightId, streamingGroup));
                            devices.Add(device);
                        }
                    }
                }

                UpdateTrigger.Start();
                Devices = new ReadOnlyCollection<IRGBDevice>(devices);
                IsInitialized = true;
            }
            catch
            {
                if (throwExceptions) throw;
                return false;
            }

            return true;
        }

        public void Dispose()
        {
            UpdateTrigger.Dispose();
        }

        public void ResetDevices()
        {
            // What should we do here, clear our devices list and start over?
            // If we close the clients in our UpdateTrigger Hue will reset the lighting to its original state,
            // depending on how the entertainment room is configured.
        }
    }
}