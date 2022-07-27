using System.Diagnostics;
using System.Drawing;
using System.IO;
using Artemis.Core;
using Artemis.Core.Modules;
using Artemis.Core.Services;
using Artemis.Plugins.Modules.Processes.Services;
using SkiaSharp;

namespace Artemis.Plugins.Modules.Processes.DataModels
{
    public class WindowDataModel
    {
        private readonly IWindowService _windowService;
        public WindowDataModel(Process process, IColorQuantizerService quantizerService, IWindowService windowService)
        {
            Process = process;
            ProcessName = process.ProcessName;

            // Accessing MainModule requires admin privileges, this way does not
            ProgramLocation = process.GetProcessFilename();

            // Get Icon colors
            if (!File.Exists(ProgramLocation))
                return;
            using MemoryStream stream = new();
            Icon.ExtractAssociatedIcon(ProgramLocation)?.Save(stream);
            stream.Seek(0, SeekOrigin.Begin);
            using SKBitmap bitmap = SKBitmap.FromImage(SKImage.FromEncodedData(stream));
            stream.Close();
            if (bitmap == null)
                return;

            SKColor[] colors = quantizerService.Quantize(bitmap.Pixels, 256);
            Colors = quantizerService.FindAllColorVariations(colors, true);

            _windowService = windowService;
        }

        public void UpdateWindowTitle()
        {
            WindowTitle = _windowService.GetActiveWindowTitle();
        }

        [DataModelIgnore]
        public Process Process { get; }

        public string WindowTitle { get; set; }
        public string ProcessName { get; set; }
        public string ProgramLocation { get; set; }
        public ColorSwatch Colors { get; set; }
    }
}