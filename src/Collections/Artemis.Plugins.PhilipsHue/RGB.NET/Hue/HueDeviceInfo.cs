﻿using System.Collections.Generic;
using System.Linq;
using Q42.HueApi;
using Q42.HueApi.Models.Groups;
using RGB.NET.Core;

namespace Artemis.Plugins.PhilipsHue.RGB.NET.Hue;

public class HueDeviceInfo : IRGBDeviceInfo
{
    public HueDeviceInfo(Group entertainmentGroup, string lightId, IEnumerable<Light> lights)
    {
        EntertainmentGroupId = entertainmentGroup.Id;
        LightId = lightId;

        Light light = lights.FirstOrDefault(l => l.Id == LightId);
        if (light == null)
            return;

        DeviceType = RGBDeviceType.Unknown;
        DeviceName = light.Name;
        Manufacturer = light.ManufacturerName;
        Model = light.ModelId;
    }

    public string EntertainmentGroupId { get; }
    public string LightId { get; }

    public RGBDeviceType DeviceType { get; }
    public string DeviceName { get; }
    public string Manufacturer { get; }
    public string Model { get; }
    public object LayoutMetadata { get; set; }
}