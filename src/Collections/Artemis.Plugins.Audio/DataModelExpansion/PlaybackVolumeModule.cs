using System;
using System.Collections.Generic;
using Artemis.Core;
using Artemis.Core.Modules;
using Artemis.Plugins.Audio.DataModelExpansion.DataModels;
using Artemis.Plugins.Audio.Services;
using NAudio.CoreAudioApi;
using Serilog;

namespace Artemis.Plugins.Audio.DataModelExpansion
{
    [PluginFeature]
    public class PlaybackVolumeModule : AudioEndpointVolumeModule
    {
        #region Constructor

        public PlaybackVolumeModule(ILogger logger, NAudioDeviceEnumerationService naudioDeviceEnumerationService, NAudio.CoreAudioApi.Role role = Role.Console) : base(logger, naudioDeviceEnumerationService, role, NAudio.CoreAudioApi.DataFlow.Render)
        {}

        #endregion

        #region Plugin Methods
        
        public override DataModelPropertyAttribute GetDataModelDescription()
        {
            return new DataModelPropertyAttribute
            {
                Name = "Playback Volume",
                Description = "Contains information about default audio playback device"
            };
        }

        #endregion
    }
}