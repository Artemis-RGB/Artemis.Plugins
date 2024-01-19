#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Tmds.MDns;

// ReSharper disable once CheckNamespace
namespace RGB.NET.Devices.WLED;

public static class WledDiscoveryHelper
{
    /// <summary>
    /// Searches for WLED devices and returns a list of devices found.
    /// </summary>
    /// <param name="waitFor">The time the discovery is waiting for responses. Choose this as short as possible as the method is blocking </param>
    /// <param name="maxDevices">The maximum amount of devices that are expected in the network. The discovery will stop early if the given amount of devices is found.</param>
    /// <returns>A list of addresses and device-infos.</returns>
    public static IEnumerable<(string address, WledInfo info)> DiscoverDevices(int waitFor = 500, int maxDevices = -1)
    {
        List<(string address, WledInfo info)> devices = [];
        using ManualResetEventSlim waitEvent = new(false);

        int devicesToDetect = maxDevices <= 0 ? int.MaxValue : maxDevices;

        ServiceBrowser mdns = new();
        mdns.ServiceAdded += OnServiceAdded;
        mdns.StartBrowse("_http._tcp");

        waitEvent.Wait(TimeSpan.FromMilliseconds(waitFor));

        mdns.StopBrowse();
        mdns.ServiceAdded -= OnServiceAdded;

        return devices;

        void OnServiceAdded(object? sender, ServiceAnnouncementEventArgs args)
        {
            string address = args.Announcement.Addresses.FirstOrDefault()?.ToString() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(address))
            {
                WledInfo? info = null;
                try
                {
                    info = WledAPI.Info(address);
                }
                catch { }

                if (info != null)
                {
                    devices.Add((address, info));
                    if (--devicesToDetect <= 0)
                        waitEvent.Set();
                }
            }
        }
    }
}