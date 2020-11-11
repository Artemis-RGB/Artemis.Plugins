using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Artemis.Core;
using Artemis.Core.DataModelExpansions;
using Artemis.Plugins.PhilipsHue.DataModels;
using Artemis.Plugins.PhilipsHue.Models;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models;
using Q42.HueApi.Models.Bridge;
using Q42.HueApi.Models.Groups;
using Serilog;

namespace Artemis.Plugins.PhilipsHue
{
    public class PluginDataModelExpansion : DataModelExpansion<HueDataModel>
    {
        private readonly ILogger _logger;
        private readonly PluginSetting<int> _pollingRateSetting;
        private readonly PluginSetting<List<PhilipsHueBridge>> _storedBridgesSetting;

        private TimedUpdateRegistration _groupsTimedUpdate;
        private TimedUpdateRegistration _hueTimedUpdate;

        public PluginDataModelExpansion(PluginSettings settings, ILogger logger)
        {
            _logger = logger;
            _storedBridgesSetting = settings.GetSetting("Bridges", new List<PhilipsHueBridge>());
            _pollingRateSetting = settings.GetSetting("PollingRate", 2);
        }

        public override DataModelPropertyAttribute GetDataModelDescription()
        {
            return new DataModelPropertyAttribute
            {
                Name = "Philips Hue",
                Description = "A data model containing all your Philips Hue bridges and their systems"
            };
        }

        public override void Enable()
        {
            Task.Run(EnablePluginAsync);

            _storedBridgesSetting.SettingSaved += StoredBridgesSettingOnSettingSaved;
            _pollingRateSetting.SettingSaved += PollingRateSettingOnSettingSaved;
        }

        public async Task EnablePluginAsync()
        {
            await UpdateExistingBridges();
            await ConnectToBridges();

            SetupTimedUpdate();
        }

        public override void Disable()
        {
            _storedBridgesSetting.SettingSaved -= StoredBridgesSettingOnSettingSaved;
            _pollingRateSetting.SettingSaved -= PollingRateSettingOnSettingSaved;
        }

        public override void Update(double deltaTime)
        {
        }

        private void StoredBridgesSettingOnSettingSaved(object? sender, EventArgs e)
        {
            ConnectToBridges();
        }

        private void PollingRateSettingOnSettingSaved(object? sender, EventArgs e)
        {
            SetupTimedUpdate();
        }

        private void SetupTimedUpdate()
        {
            _groupsTimedUpdate?.Stop();
            _groupsTimedUpdate = AddTimedUpdate(TimeSpan.FromMinutes(1), UpdateGroups);
            _hueTimedUpdate?.Stop();
            _hueTimedUpdate = AddTimedUpdate(TimeSpan.FromSeconds(_pollingRateSetting.Value), UpdateHue);
        }

        private async Task UpdateGroups(double delta)
        {
            foreach (PhilipsHueBridge bridge in _storedBridgesSetting.Value)
            {
                // Add or update current groups
                List<Group> groups = (await bridge.Client.GetGroupsAsync()).Where(g => g.Type == GroupType.Room || g.Type == GroupType.Zone).ToList();
                DataModel.Rooms.Update(bridge, groups);
                DataModel.Zones.Update(bridge, groups);
            }
        }

        private async Task UpdateHue(double delta)
        {
            if (!DataModel.Rooms.Groups.Any() && !DataModel.Zones.Groups.Any())
                await UpdateGroups(delta);

            foreach (PhilipsHueBridge bridge in _storedBridgesSetting.Value)
            {
                List<Light> lights = (await bridge.Client.GetLightsAsync()).ToList();
                List<Sensor> sensors = (await bridge.Client.GetSensorsAsync()).Where(s => s.Capabilities != null).ToList();

                DataModel.Rooms.UpdateContents(bridge, lights);
                DataModel.Zones.UpdateContents(bridge, lights);

                DataModel.Accessories.UpdateContents(bridge, sensors);
            }
        }

        #region Bridge management

        public async Task UpdateExistingBridges()
        {
            IBridgeLocator locator = new HttpBridgeLocator();
            List<LocatedBridge> bridges = (await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5))).ToList();

            // Lets try to find some more 
            locator = new SsdpBridgeLocator();
            IEnumerable<LocatedBridge> extraBridges = await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5));
            foreach (LocatedBridge extraBridge in extraBridges)
                if (bridges.All(b => b.BridgeId != extraBridge.BridgeId))
                    bridges.Add(extraBridge);

            int updatedBridges = 0;
            foreach (LocatedBridge locatedBridge in bridges)
            {
                PhilipsHueBridge storedBridge = _storedBridgesSetting.Value.FirstOrDefault(s => s.BridgeId == locatedBridge.BridgeId);
                if (storedBridge != null && storedBridge.IpAddress != locatedBridge.IpAddress)
                {
                    storedBridge.IpAddress = locatedBridge.IpAddress;
                    updatedBridges++;
                }
            }

            if (updatedBridges > 0)
            {
                _storedBridgesSetting.Save();
                _logger.Information("Updated IP addresses of {updatedBridges} Hue Bridge(s)", updatedBridges);
            }
        }

        private async Task ConnectToBridges()
        {
            DataModel.ClearDynamicChildren();
            foreach (PhilipsHueBridge philipsHueBridge in _storedBridgesSetting.Value)
            {
                ILocalHueClient client = new LocalHueClient(philipsHueBridge.IpAddress);
                client.Initialize(philipsHueBridge.AppKey);

                bool success = await client.CheckConnection();
                if (success)
                {
                    Bridge bridgeInfo = await client.GetBridgeAsync();
                    philipsHueBridge.Client = client;
                    philipsHueBridge.BridgeInfo = bridgeInfo;
                    _logger.Information("Connected to Hue bridge at {ip}", philipsHueBridge.IpAddress);
                }
                else
                {
                    _logger.Warning("Failed to connect to Hue bridge at {ip}", philipsHueBridge.IpAddress);
                }
            }
        }

        #endregion
    }
}