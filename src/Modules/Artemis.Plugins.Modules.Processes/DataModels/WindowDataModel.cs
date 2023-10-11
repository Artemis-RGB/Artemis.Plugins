using System.Diagnostics;
using System.Drawing;
using System.IO;
using Artemis.Core;
using Artemis.Core.ColorScience;
using Artemis.Core.Modules;
using Artemis.Core.Services;
using Artemis.Plugins.Modules.Processes.Services.Windows.WindowServices;
using SkiaSharp;

namespace Artemis.Plugins.Modules.Processes.DataModels;

public class WindowDataModel
{
    [DataModelProperty(Description = "The title of the window")]
    public string WindowTitle { get; set; }
    
    [DataModelProperty(Description = "The process name of the active window, does not include .exe")]
    public string ProcessName { get; set; }
    
    [DataModelProperty(Description = "The full executable path of the active window")]
    public string ProgramLocation { get; set; }

    [DataModelProperty(Description = "Whether or not the active window is full screen on the main monitor")]
    public bool IsFullscreen { get; set; }

    [DataModelProperty(Description = "The colors of the icon of the window")]
    public ColorSwatch Colors { get; set; }
}