using System.Diagnostics;
using System.Drawing;
using System.IO;
using Artemis.Core;
using Artemis.Core.Modules;
using Artemis.Core.Services;
using Artemis.Plugins.Modules.Processes.Services.Windows.WindowServices;
using SkiaSharp;

namespace Artemis.Plugins.Modules.Processes.DataModels;

public class WindowDataModel
{
    public string WindowTitle { get; set; }
    public string ProcessName { get; set; }
    public string ProgramLocation { get; set; }
    public ColorSwatch Colors { get; set; }
}