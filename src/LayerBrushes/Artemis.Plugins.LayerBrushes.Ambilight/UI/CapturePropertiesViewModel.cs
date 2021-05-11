using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Artemis.Core;
using Artemis.Core.LayerBrushes;
using Artemis.Plugins.LayerBrushes.Ambilight.PropertyGroups;
using Artemis.UI.Shared;
using Artemis.UI.Shared.LayerBrushes;
using FluentValidation;
using ScreenCapture;
using SkiaSharp;
using Stylet;

namespace Artemis.Plugins.LayerBrushes.Ambilight.UI
{
    public class CapturePropertiesViewModel : BrushConfigurationViewModel
    {
        #region Constructors

        public CapturePropertiesViewModel(BaseLayerBrush layerBrush, IModelValidator<CapturePropertiesViewModel> validator)
            : base(layerBrush, validator)
        {
            _properties = ((AmbilightLayerBrush) layerBrush).Properties;
        }

        #endregion

        #region Properties & Fields

        private IScreenCaptureService _screenCaptureService => AmbilightBootstrapper.ScreenCaptureService;

        private readonly AmbilightPropertyGroup _properties;

        private readonly Timer _displayPreviewTimer = new(500) {AutoReset = true};
        private readonly Timer _selectionPreviewTimer = new(33) {AutoReset = true};

        private bool _preventPreviewCreation;

        public List<DisplayPreview> Displays { get; private set; }

        private DisplayPreview _selectedDisplay;

        public DisplayPreview SelectedDisplay
        {
            get => _selectedDisplay;
            set
            {
                if (SetAndNotify(ref _selectedDisplay, value))
                {
                    if (RegionX + RegionWidth > (value?.Display.Width ?? 0) || RegionY + RegionHeight > (value?.Display.Height ?? 0) || RegionWidth == 0 || RegionHeight == 0)
                    {
                        _preventPreviewCreation = true;
                        RegionX = 0;
                        RegionY = 0;
                        RegionWidth = value?.Display.Width ?? 0;
                        RegionHeight = value?.Display.Height ?? 0;
                        _preventPreviewCreation = false;
                    }

                    RecreatePreview();
                }
            }
        }

        public DisplayPreview Preview { get; private set; }

        private int _regionX;

        public int RegionX
        {
            get => _regionX;
            set
            {
                if (SetAndNotify(ref _regionX, value))
                    RecreatePreview();
            }
        }

        private int _regionY;

        public int RegionY
        {
            get => _regionY;
            set
            {
                if (SetAndNotify(ref _regionY, value))
                    RecreatePreview();
            }
        }

        private int _regionWidth;

        public int RegionWidth
        {
            get => _regionWidth;
            set
            {
                if (SetAndNotify(ref _regionWidth, value))
                    RecreatePreview();
            }
        }

        private int _regionHeight;

        public int RegionHeight
        {
            get => _regionHeight;
            set
            {
                if (SetAndNotify(ref _regionHeight, value))
                    RecreatePreview();
            }
        }

        private bool _regionFlipHorizontal;

        public bool RegionFlipHorizontal
        {
            get => _regionFlipHorizontal;
            set
            {
                if (SetAndNotify(ref _regionFlipHorizontal, value))
                    RecreatePreview();
            }
        }

        private bool _regionFlipVertical;

        public bool RegionFlipVertical
        {
            get => _regionFlipVertical;
            set
            {
                if (SetAndNotify(ref _regionFlipVertical, value))
                    RecreatePreview();
            }
        }

        private int _downscaleLevel;

        public int DownscaleLevel
        {
            get => _downscaleLevel;
            set
            {
                if (SetAndNotify(ref _downscaleLevel, value))
                    RecreatePreview();
            }
        }

        private bool _blackBarDetectionTop;

        public bool BlackBarDetectionTop
        {
            get => _blackBarDetectionTop;
            set
            {
                if (SetAndNotify(ref _blackBarDetectionTop, value))
                    RecreatePreview();
            }
        }

        private bool _blackBarDetectionBottom;

        public bool BlackBarDetectionBottom
        {
            get => _blackBarDetectionBottom;
            set
            {
                if (SetAndNotify(ref _blackBarDetectionBottom, value))
                    RecreatePreview();
            }
        }

        private bool _blackBarDetectionLeft;

        public bool BlackBarDetectionLeft
        {
            get => _blackBarDetectionLeft;
            set
            {
                if (SetAndNotify(ref _blackBarDetectionLeft, value))
                    RecreatePreview();
            }
        }

        private bool _blackBarDetectionRight;

        public bool BlackBarDetectionRight
        {
            get => _blackBarDetectionRight;
            set
            {
                if (SetAndNotify(ref _blackBarDetectionRight, value))
                    RecreatePreview();
            }
        }

        private int _blackBarDetectionThreshold;

        public int BlackBarDetectionThreshold
        {
            get => _blackBarDetectionThreshold;
            set
            {
                if (SetAndNotify(ref _blackBarDetectionThreshold, value))
                    RecreatePreview();
            }
        }

        #endregion

        #region Methods

        private bool _dragCenter;
        private bool _dragLeft;
        private bool _dragRight;
        private bool _dragBottom;
        private bool _dragTop;
        private Vector _centerDragOffset;

        public void RegionSelectMouseDown(object sender, MouseEventArgs args)
        {
            Canvas canvas = VisualTreeUtilities.FindParent<Canvas>((DependencyObject) sender, null);
            Point position = args.GetPosition(canvas);
            ((IInputElement) sender).CaptureMouse();
            _preventPreviewCreation = true;

            // Horizontal dragging
            if (Math.Abs(position.X - RegionX) < 60)
                _dragLeft = true;
            else if (Math.Abs(position.X - (RegionX + RegionWidth)) < 60)
                _dragRight = true;
            // Vertical dragging
            if (Math.Abs(position.Y - RegionY) < 60)
                _dragTop = true;
            else if (Math.Abs(position.Y - (RegionY + RegionHeight)) < 60)
                _dragBottom = true;
            // Movement dragging
            if (!_dragLeft && !_dragRight && !_dragLeft && !_dragTop && !_dragBottom)
            {
                _dragCenter = true;
                _centerDragOffset = new Point(RegionX, RegionY) - position;
            }
        }

        public void RegionSelectMouseUp(object sender, MouseEventArgs args)
        {
            ((IInputElement) sender).ReleaseMouseCapture();
            _preventPreviewCreation = false;

            _dragCenter = false;
            _dragLeft = false;
            _dragRight = false;
            _dragTop = false;
            _dragBottom = false;

            RecreatePreview();
        }

        public void RegionSelectMouseMove(object sender, MouseEventArgs args)
        {
            Canvas canvas = VisualTreeUtilities.FindParent<Canvas>((DependencyObject) sender, null);
            Point position = args.GetPosition(canvas);

            SKRectI region = SKRectI.Create(RegionX, RegionY, RegionWidth, RegionHeight);

            if (_dragLeft)
                region.Left = position.X.RoundToInt();
            if (_dragRight)
                region.Right = position.X.RoundToInt();
            if (_dragTop)
                region.Top = position.Y.RoundToInt();
            if (_dragBottom)
                region.Bottom = position.Y.RoundToInt();
            if (_dragCenter)
            {
                region.Location = new SKPointI((position.X + _centerDragOffset.X).RoundToInt(), (position.Y + _centerDragOffset.Y).RoundToInt());
                if (region.Left < 0)
                    region.Location = new SKPointI(0, region.Location.Y);
                if (region.Top < 0)
                    region.Location = new SKPointI(region.Location.X, 0);
                if (region.Right > SelectedDisplay.Display.Width)
                    region.Location = new SKPointI(SelectedDisplay.Display.Width - region.Width, region.Location.Y);
                if (region.Bottom > SelectedDisplay.Display.Height)
                    region.Location = new SKPointI(region.Location.X, SelectedDisplay.Display.Height - region.Height);
            }

            region.Left = Math.Max(0, region.Left);
            region.Right = Math.Min(SelectedDisplay.Display.Width, region.Right);
            region.Top = Math.Max(0, region.Top);
            region.Bottom = Math.Min(SelectedDisplay.Display.Height, region.Bottom);

            RegionX = region.Left;
            RegionY = region.Top;
            RegionWidth = region.Width;
            RegionHeight = region.Height;
        }

        protected override void OnInitialActivate()
        {
            base.OnInitialActivate();

            Displays = _screenCaptureService.GetGraphicsCards()
                .SelectMany(gg => _screenCaptureService.GetDisplays(gg))
                .Select(d => new DisplayPreview(d))
                .ToList();

            AmbilightCaptureProperties? props = _properties.Capture.BaseValue;
            if (props != null)
            {
                _preventPreviewCreation = true;
                AmbilightCaptureProperties properties = props.Value;
                SelectedDisplay = Displays.FirstOrDefault(d =>
                    d.Display.GraphicsCard.VendorId == properties.GraphicsCardVendorId &&
                    d.Display.GraphicsCard.DeviceId == properties.GraphicsCardDeviceId &&
                    d.Display.DeviceName == properties.DisplayName);
                RegionX = properties.X;
                RegionY = properties.Y;
                RegionWidth = properties.Width;
                RegionHeight = properties.Height;
                RegionFlipHorizontal = properties.FlipHorizontal;
                RegionFlipVertical = properties.FlipVertical;
                DownscaleLevel = properties.DownscaleLevel;
                BlackBarDetectionTop = properties.BlackBarDetectionTop;
                BlackBarDetectionBottom = properties.BlackBarDetectionBottom;
                BlackBarDetectionLeft = properties.BlackBarDetectionLeft;
                BlackBarDetectionRight = properties.BlackBarDetectionRight;
                BlackBarDetectionThreshold = properties.BlackBarDetectionThreshold;
                _preventPreviewCreation = false;
            }

            UpdateDisplayPreviews();
            RecreatePreview();

            _displayPreviewTimer.Elapsed += (_, _) => UpdateDisplayPreviews();
            _displayPreviewTimer.Start();

            _selectionPreviewTimer.Elapsed += (_, _) => UpdateSelectionPreview();
            _selectionPreviewTimer.Start();
        }

        private void RecreatePreview()
        {
            if (_preventPreviewCreation) return;

            Preview?.Dispose();
            Preview = SelectedDisplay == null
                ? null
                : new DisplayPreview(SelectedDisplay.Display, RegionX, RegionY, RegionWidth, RegionHeight, DownscaleLevel,
                    BlackBarDetectionTop, BlackBarDetectionBottom, BlackBarDetectionLeft, BlackBarDetectionRight, BlackBarDetectionThreshold);

            NotifyOfPropertyChange(nameof(Preview));
        }

        private void UpdateDisplayPreviews()
        {
            try
            {
                lock (Displays)
                {
                    foreach (DisplayPreview preview in Displays)
                        preview.Update();
                }
            }
            catch
            {
                // ignored
            }
        }

        private void UpdateSelectionPreview()
        {
            try
            {
                Preview?.Update();
            }
            catch
            {
                // ignored
            }
        }

        private void CommitChanges()
        {
            if (Validate())
            {
                _properties.Capture.BaseValue = new AmbilightCaptureProperties(SelectedDisplay.Display, RegionX, RegionY, RegionWidth, RegionHeight, RegionFlipHorizontal, RegionFlipVertical,
                    DownscaleLevel,
                    BlackBarDetectionTop, BlackBarDetectionBottom, BlackBarDetectionLeft, BlackBarDetectionRight, BlackBarDetectionThreshold);
            }
        }

        protected override void OnClose()
        {
            _preventPreviewCreation = true;

            _displayPreviewTimer.Stop();
            _displayPreviewTimer.Dispose();

            _selectionPreviewTimer.Stop();
            _selectionPreviewTimer.Dispose();

            lock (Displays)
            {
                foreach (DisplayPreview display in Displays)
                    display.Dispose();

                Displays.Clear();
            }

            Preview?.Dispose();

            CommitChanges();

            base.OnClose();
        }

        #endregion
    }

    #region Data

    public sealed class DisplayPreview : PropertyChangedBase, IDisposable
    {
        #region Properties & Fields

        private bool _isDisposed;

        private readonly CaptureZone _captureZone;
        private readonly CaptureZone _processedCaptureZone;

        private readonly bool _blackBarDetectionTop;
        private readonly bool _blackBarDetectionBottom;
        private readonly bool _blackBarDetectionLeft;
        private readonly bool _blackBarDetectionRight;

        public Display Display { get; }

        private readonly byte[] _previewBuffer;
        private readonly byte[] _processedPreviewBuffer;

        private WriteableBitmap _preview;

        public WriteableBitmap Preview
        {
            get => _preview;
            set => SetAndNotify(ref _preview, value);
        }

        private WriteableBitmap _processedPreview;

        public WriteableBitmap ProcessedPreview
        {
            get => _processedPreview;
            set => SetAndNotify(ref _processedPreview, value);
        }

        #endregion

        #region Constructors

        public DisplayPreview(Display display, bool highQuality = false)
        {
            Display = display;

            _captureZone = AmbilightBootstrapper.ScreenCaptureService.GetScreenCapture(display).RegisterCaptureZone(0, 0, display.Width, display.Height, highQuality ? 0 : 2);

            _previewBuffer = new byte[_captureZone.Buffer.Length];
            Preview = new WriteableBitmap(BitmapSource.Create(_captureZone.Width, _captureZone.Height, 96, 96, PixelFormats.Bgra32, null, _previewBuffer, _captureZone.BufferWidth * 4));
        }

        public DisplayPreview(Display display, int x, int y, int width, int height, int downsamplingLevel,
            bool blackBarDetectionTop, bool blackBarDetectionBottom, bool blackBarDetectionLeft, bool blackBarDetectionRight, int blackBarThreshold)
        {
            Display = display;
            _blackBarDetectionBottom = blackBarDetectionBottom;
            _blackBarDetectionTop = blackBarDetectionTop;
            _blackBarDetectionLeft = blackBarDetectionLeft;
            _blackBarDetectionRight = blackBarDetectionRight;

            _captureZone = AmbilightBootstrapper.ScreenCaptureService.GetScreenCapture(display).RegisterCaptureZone(0, 0, display.Width, display.Height);
            _previewBuffer = new byte[_captureZone.Buffer.Length];
            Preview = new WriteableBitmap(BitmapSource.Create(_captureZone.Width, _captureZone.Height, 96, 96, PixelFormats.Bgra32, null, _previewBuffer, _captureZone.BufferWidth * 4));

            if (x + width <= display.Width && y + height <= display.Height)
            {
                _processedCaptureZone = AmbilightBootstrapper.ScreenCaptureService.GetScreenCapture(display).RegisterCaptureZone(x, y, width, height, downsamplingLevel);
                _processedCaptureZone.BlackBars.Threshold = blackBarThreshold;
                _processedPreviewBuffer = new byte[_processedCaptureZone.Buffer.Length];
                ProcessedPreview = new WriteableBitmap(
                    BitmapSource.Create(_processedCaptureZone.Width, _processedCaptureZone.Height, 96, 96, PixelFormats.Bgra32, null, _processedPreviewBuffer, _processedCaptureZone.BufferWidth * 4)
                );
            }
        }

        #endregion

        #region Methods

        public void Update()
        {
            Execute.OnUIThreadSync(() =>
            {
                if (_isDisposed) return;

                lock (_captureZone.Buffer)
                {
                    Preview.WritePixels(new Int32Rect(0, 0, _captureZone.Width, _captureZone.Height), _captureZone.Buffer, _captureZone.BufferWidth * 4, 0, 0);
                }

                if (_processedCaptureZone == null)
                    return;

                lock (_processedCaptureZone.Buffer)
                {
                    if (_processedCaptureZone.Buffer.Length == 0) return;

                    if (_blackBarDetectionTop || _blackBarDetectionBottom || _blackBarDetectionLeft || _blackBarDetectionRight)
                    {
                        int x = _blackBarDetectionLeft ? _processedCaptureZone.BlackBars.Left : 0;
                        int y = _blackBarDetectionTop ? _processedCaptureZone.BlackBars.Top : 0;
                        int width = _processedCaptureZone.Width - (_blackBarDetectionRight ? _processedCaptureZone.BlackBars.Right : 0) - x;
                        int height = _processedCaptureZone.Height - (_blackBarDetectionBottom ? _processedCaptureZone.BlackBars.Bottom : 0) - y;

                        if ((ProcessedPreview.PixelWidth != width || ProcessedPreview.PixelHeight != height) && width > 0 && height > 0)
                            ProcessedPreview = new WriteableBitmap(
                                BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgra32, null, _processedPreviewBuffer, _processedCaptureZone.BufferWidth * 4));

                        ProcessedPreview.WritePixels(new Int32Rect(x, y, width, height), _processedCaptureZone.Buffer, _processedCaptureZone.BufferWidth * 4, 0, 0);
                    }
                    else
                        ProcessedPreview.WritePixels(new Int32Rect(0, 0, _processedCaptureZone.Width, _processedCaptureZone.Height), _processedCaptureZone.Buffer,
                            _processedCaptureZone.BufferWidth * 4, 0, 0);
                }
            });
        }

        public void Dispose()
        {
            AmbilightBootstrapper.ScreenCaptureService.GetScreenCapture(Display).UnregisterCaptureZone(_captureZone);
            _isDisposed = true;
        }

        #endregion
    }

    #endregion

    #region Validation

    public class CapturePropertiesViewModelValidator : AbstractValidator<CapturePropertiesViewModel>
    {
        #region Constructors

        public CapturePropertiesViewModelValidator()
        {
            RuleFor(vm => vm.SelectedDisplay).NotNull().WithName("Display").WithMessage("No display selected.");
            RuleFor(vm => vm.RegionX).GreaterThanOrEqualTo(0).WithName("X").WithMessage("X needs to be 0 or greater");
            RuleFor(vm => vm.RegionY).GreaterThanOrEqualTo(0).WithName("Y").WithMessage("Y needs to be 0 or greater");
            RuleFor(vm => vm.RegionWidth).GreaterThan(0).WithName("Width").WithMessage("Width needs to be greater than 0");
            RuleFor(vm => vm.RegionHeight).GreaterThan(0).WithName("Height").WithMessage("Height needs to be greater than 0");
            RuleFor(vm => vm.RegionX + vm.RegionWidth).LessThanOrEqualTo(vm => vm.SelectedDisplay.Display.Width).WithName("X/Width").WithMessage("The region exceeds the display width.");
            RuleFor(vm => vm.RegionY + vm.RegionHeight).LessThanOrEqualTo(vm => vm.SelectedDisplay.Display.Height).WithName("Y/Height").WithMessage("The region exceeds the display height.");
        }

        #endregion
    }

    #endregion
}