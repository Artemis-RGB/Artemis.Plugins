using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Artemis.Core;
using Artemis.Core.Modules;
using Artemis.Core.Services;
using Artemis.Plugins.Modules.General.Utilities;
using SkiaSharp;

namespace Artemis.Plugins.Modules.General.DataModels.Windows
{
    public class WindowDataModel
    {
        public WindowDataModel(Process process, IColorQuantizerService quantizerService)
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

            List<SKColor> colors = quantizerService.Quantize(bitmap.Pixels.ToList(), 256).ToList();
            Colors = new IconColorsDataModel
            {
                Vibrant = quantizerService.FindColorVariation(colors, ColorType.Vibrant, true),
                LightVibrant = quantizerService.FindColorVariation(colors, ColorType.LightVibrant, true),
                DarkVibrant = quantizerService.FindColorVariation(colors, ColorType.DarkVibrant, true),
                Muted = quantizerService.FindColorVariation(colors, ColorType.Muted, true),
                LightMuted = quantizerService.FindColorVariation(colors, ColorType.LightMuted, true),
                DarkMuted = quantizerService.FindColorVariation(colors, ColorType.DarkMuted, true)
            };
        }

        public void UpdateWindowTitle()
        {
            WindowTitle = WindowUtilities.GetActiveWindowTitle();
        }

        [DataModelIgnore]
        public Process Process { get; }

        public string WindowTitle { get; set; }
        public string ProcessName { get; set; }
        public string ProgramLocation { get; set; }
        public IconColorsDataModel Colors { get; set; }
    }
}