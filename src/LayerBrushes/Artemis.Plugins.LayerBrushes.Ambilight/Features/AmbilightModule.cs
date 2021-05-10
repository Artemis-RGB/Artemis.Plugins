using System.Linq;
using Artemis.Core;
using Artemis.Core.Services;
using Microsoft.Win32;
using Serilog;

namespace Artemis.Plugins.LayerBrushes.Ambilight.Features
{
    [PluginFeature(Name = "Ambilight auto restart", Description = "This module the ambilight plugin the ability to auto restart if windows display settings is changed", Icon = "Restart", AlwaysEnabled = true)]
    public class AmbilightModule : PluginFeature
    {
        private readonly IPluginManagementService _pluginManagementService;
        private readonly ILogger _logger;

        public AmbilightModule(IPluginManagementService pluginManagementService, ILogger logger)
        {
            _pluginManagementService = pluginManagementService;
            _logger = logger;
        }

        public override void Enable()
        {
            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
        }

        public override void Disable()
        {
            SystemEvents.DisplaySettingsChanged -= SystemEvents_DisplaySettingsChanged;
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, System.EventArgs e)
        {
            _logger.Verbose("Display setting change event.");

            PluginFeatureInfo? layerBrushFeature = Plugin.Features.FirstOrDefault(f => f.FeatureType == typeof(AmbilightLayerBrushProvider));

            // Restart layer feature feature only if it is already enabled
            bool restartFeature = layerBrushFeature?.Instance != null && (layerBrushFeature?.Instance.IsEnabled ?? false);
            if (restartFeature) _logger.Verbose("Restarting Ambilight Layerbrush Feature."); else _logger.Verbose("Ambilight Layerbrush Feature not enabled. Nothing to be restarted");
            ;
            if (restartFeature) _pluginManagementService.DisablePluginFeature(layerBrushFeature.Instance, false);

            // Restart service to refresh factory and all DX internals.Needed to detect new displays or display setting changes
            AmbilightBootstrapper.RefreshScreenCaptureService();

            if (restartFeature) _pluginManagementService.EnablePluginFeature(layerBrushFeature.Instance, false);
        }
    }
}