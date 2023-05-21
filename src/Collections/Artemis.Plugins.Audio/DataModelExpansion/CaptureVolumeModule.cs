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
    public class CaptureVolumeModule : AudioEndpointVolumeModule
    {
        #region Constructor

        public CaptureVolumeModule(ILogger logger, NAudioDeviceEnumerationService naudioDeviceEnumerationService, NAudio.CoreAudioApi.Role role = Role.Communications) : base(logger, naudioDeviceEnumerationService, role, NAudio.CoreAudioApi.DataFlow.Capture)
        {}

        #endregion

        #region Plugin Methods
        
        public override DataModelPropertyAttribute GetDataModelDescription()
        {
            return new DataModelPropertyAttribute
            {
                Name = "Capture Volume",
                Description = "Contains information about default communication capture device"
            };
        }

        #endregion
    }
}