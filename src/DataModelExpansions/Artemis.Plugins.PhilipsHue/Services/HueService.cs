using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Artemis.Core;
using Artemis.Core.Services;
using Artemis.Plugins.PhilipsHue.Models;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Bridge;
using Serilog;

namespace Artemis.Plugins.PhilipsHue.Services
{
    public class HueService : IHueService
    {
        private readonly ILogger _logger;
        private readonly PluginSetting<List<PhilipsHueBridge>> _storedBridgesSetting;

        public HueService(ILogger logger, PluginSettings settings)
        {
            _logger = logger;
            _storedBridgesSetting = settings.GetSetting("Bridges", new List<PhilipsHueBridge>());
        }

        #region IDisposable

        public void Dispose()
        {
        }

        #endregion
        
        #region Bridge management

        public List<PhilipsHueBridge> Bridges => _storedBridgesSetting.Value;

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

        public async Task ConnectToBridges()
        {
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

    public interface IHueService : IPluginService, IDisposable
    {
        List<PhilipsHueBridge> Bridges { get; }
        Task UpdateExistingBridges();
        Task ConnectToBridges();
    }
}