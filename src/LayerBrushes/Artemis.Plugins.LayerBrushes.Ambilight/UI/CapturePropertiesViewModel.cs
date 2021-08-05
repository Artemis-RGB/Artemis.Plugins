using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using Artemis.Core;
using Artemis.Core.LayerBrushes;
using Artemis.Plugins.LayerBrushes.Ambilight.PropertyGroups;
using Artemis.UI.Shared;
using Artemis.UI.Shared.LayerBrushes;
using FluentValidation;
using ScreenCapture.NET;
using SkiaSharp;
using Stylet;

namespace Artemis.Plugins.LayerBrushes.Ambilight.UI
{
    public class CapturePropertiesViewModel : BrushConfigurationViewModel
    {
        #region Constructors

        public CapturePropertiesViewModel(BaseLayerBrush layerBrush, IModelValidator<CapturePropertiesViewModel> validator) : base(layerBrush, validator)
        {
            Properties = ((AmbilightLayerBrush) layerBrush).Properties.Capture;
        }

        #endregion

        #region Properties & Fields

        private IScreenCaptureService _screenCaptureService => AmbilightBootstrapper.ScreenCaptureService;
        private readonly Timer _displayPreviewTimer = new(500) {AutoReset = true};
        private readonly Timer _selectionPreviewTimer = new(33) {AutoReset = true};
        private bool _preventPreviewCreation;
        private DisplayPreview _selectedDisplay;

        public int X
        {
            get => Properties.X.BaseValue;
            set
            {
                Properties.X.BaseValue = value;
                NotifyOfPropertyChange(nameof(X));
            }
        }

        public int Y
        {
            get => Properties.Y.BaseValue;
            set
            {
                Properties.Y.BaseValue = value;
                NotifyOfPropertyChange(nameof(Y));
            }
        }

        public int Width
        {
            get => Properties.Width.BaseValue;
            set
            {
                Properties.Width.BaseValue = value;
                NotifyOfPropertyChange(nameof(Width));
            }
        }

        public int Height
        {
            get => Properties.Height.BaseValue;
            set
            {
                Properties.Height.BaseValue = value;
                NotifyOfPropertyChange(nameof(Height));
            }
        }

        public SKRectI Region => SKRectI.Create(X, Y, Width, Height);
        public AmbilightCaptureProperties Properties { get; }
        public BindableCollection<DisplayPreview> Displays { get; } = new();

        public DisplayPreview SelectedDisplay
        {
            get => _selectedDisplay;
            set
            {
                if (!IsActive) return; // Don't update display and region if dialog is being closed to avoid 0 values
                if (!SetAndNotify(ref _selectedDisplay, value)) return;
                if (X + Width > (value?.Display.Width ?? 0) || Y + Height > (value?.Display.Height ?? 0) || Width == 0 || Height == 0)
                {
                    _preventPreviewCreation = true;
                    X = 0;
                    Y = 0;
                    Width = value?.Display.Width ?? 0;
                    Height = value?.Display.Height ?? 0;
                    _preventPreviewCreation = false;
                }

                RecreatePreview();
            }
        }

        public DisplayPreview Preview { get; private set; }

        #endregion

        #region Methods

        private bool _dragCenter;
        private bool _dragLeft;
        private bool _dragRight;
        private bool _dragBottom;
        private bool _dragTop;
        private Vector _centerDragOffset;

        public void ResetRegion()
        {
            _preventPreviewCreation = true;
            X = 0;
            Y = 0;
            Width = SelectedDisplay?.Display.Width ?? 0;
            Height = SelectedDisplay?.Display.Height ?? 0;
            _preventPreviewCreation = false;

            RecreatePreview();
        }

        public void RegionSelectMouseDown(object sender, MouseEventArgs args)
        {
            if (sender is not Shape shape)
                return;

            Canvas canvas = VisualTreeUtilities.FindParent<Canvas>(shape, null);
            Point position = args.GetPosition(canvas);
            shape.CaptureMouse();
            _preventPreviewCreation = true;

            // Detect control points
            _dragTop = shape.Name.Contains("Top");
            _dragBottom = shape.Name.Contains("Bottom");
            _dragLeft = shape.Name.Contains("Left");
            _dragRight = shape.Name.Contains("Right");
            if (_dragLeft || _dragRight || _dragLeft || _dragTop || _dragBottom)
                return;

            // No control points means move the region instead of resize
            _dragCenter = true;
            _centerDragOffset = new Point(X, Y) - position;
        }

        public void RegionSelectMouseUp(object sender, MouseEventArgs args)
        {
            ((Shape) sender).ReleaseMouseCapture();
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
            if (args.LeftButton == MouseButtonState.Released) return;

            Canvas canvas = VisualTreeUtilities.FindParent<Canvas>((Shape) sender, null);
            Point position = args.GetPosition(canvas);

            SKRectI region = SKRectI.Create(X, Y, Width, Height);

            if (_dragLeft)
                region.Left = Math.Min(region.Right - 1, position.X.RoundToInt());
            if (_dragRight)
                region.Right = position.X.RoundToInt();
            if (_dragTop)
                region.Top = Math.Min(region.Bottom - 1, position.Y.RoundToInt());
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

            X = Math.Max(0, region.Left);
            Y = Math.Max(0, region.Top);
            Width = Math.Max(1, region.Width);
            Height = Math.Max(1, region.Height);
        }

        private void RecreatePreview()
        {
            NotifyOfPropertyChange(nameof(Region));

            if (_preventPreviewCreation)
                return;

            if (SelectedDisplay == null)
            {
                Preview?.Dispose();
                Preview = null;
            }

            if (Validate())
            {
                Preview?.Dispose();
                Preview = new DisplayPreview(SelectedDisplay.Display, Properties);
            }

            NotifyOfPropertyChange(nameof(Preview));
        }

        private void UpdateDisplayPreviews()
        {
            Execute.OnUIThreadSync(() =>
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
            });
        }

        private void UpdateSelectionPreview()
        {
            Execute.OnUIThreadSync(() =>
            {
                try
                {
                    Preview?.Update();
                }
                catch
                {
                    // ignored
                }
            });
        }

        public override Task<bool> CanCloseAsync()
        {
            return ValidateAsync();
        }

        protected override void OnInitialActivate()
        {
            base.OnInitialActivate();

            ((AmbilightLayerBrush) LayerBrush).PropertiesOpen = true;

            Displays.AddRange(_screenCaptureService.GetGraphicsCards()
                .SelectMany(gg => _screenCaptureService.GetDisplays(gg))
                .Select(d => new DisplayPreview(d))
                .ToList());

            _preventPreviewCreation = true;
            SelectedDisplay = Displays.FirstOrDefault(d =>
                d.Display.GraphicsCard.VendorId == Properties.GraphicsCardVendorId &&
                d.Display.GraphicsCard.DeviceId == Properties.GraphicsCardDeviceId &&
                d.Display.DeviceName == Properties.DisplayName.BaseValue);

            _preventPreviewCreation = false;

            UpdateDisplayPreviews();
            RecreatePreview();

            _displayPreviewTimer.Elapsed += (_, _) => UpdateDisplayPreviews();
            _displayPreviewTimer.Start();

            _selectionPreviewTimer.Elapsed += (_, _) => UpdateSelectionPreview();
            _selectionPreviewTimer.Start();

            Properties.LayerPropertyOnCurrentValueSet += PropertiesOnLayerPropertyOnCurrentValueSet;
        }

        private void PropertiesOnLayerPropertyOnCurrentValueSet(object sender, LayerPropertyEventArgs e)
        {
            RecreatePreview();
        }

        protected override void OnClose()
        {
            _preventPreviewCreation = true;

            _displayPreviewTimer.Stop();
            _displayPreviewTimer.Dispose();

            _selectionPreviewTimer.Stop();
            _selectionPreviewTimer.Dispose();

            Properties.LayerPropertyOnCurrentValueSet -= PropertiesOnLayerPropertyOnCurrentValueSet;
            Properties.ApplyDisplay(SelectedDisplay.Display, false);

            lock (Displays)
            {
                foreach (DisplayPreview display in Displays)
                    display.Dispose();

                Displays.Clear();
            }

            Preview?.Dispose();

            ((AmbilightLayerBrush) LayerBrush).PropertiesOpen = false;
            ((AmbilightLayerBrush) LayerBrush).RecreateCaptureZone();

            base.OnClose();
        }

        #endregion
    }

    #region Data

    #endregion

    #region Validation

    public class CapturePropertiesViewModelValidator : AbstractValidator<CapturePropertiesViewModel>
    {
        #region Constructors

        public CapturePropertiesViewModelValidator()
        {
            RuleFor(vm => vm.X).GreaterThanOrEqualTo(0).WithName("X").WithMessage("X needs to be 0 or greater");
            RuleFor(vm => vm.Y).GreaterThanOrEqualTo(0).WithName("Y").WithMessage("Y needs to be 0 or greater");
            RuleFor(vm => vm.Width).GreaterThan(0).WithName("Width").WithMessage("Width needs to be greater than 0");
            RuleFor(vm => vm.Height).GreaterThan(0).WithName("Height").WithMessage("Height needs to be greater than 0");
            RuleFor(vm => vm.SelectedDisplay).NotNull().WithName("Display").WithMessage("No display selected.").DependentRules(() =>
            {
                RuleFor(vm => vm.X + vm.Width).LessThanOrEqualTo(vm => vm.SelectedDisplay.Display.Width).WithName("X/Width").WithMessage("The region exceeds the display width.");
                RuleFor(vm => vm.Y + vm.Height).LessThanOrEqualTo(vm => vm.SelectedDisplay.Display.Height).WithName("Y/Height").WithMessage("The region exceeds the display height.");
            });
        }

        #endregion
    }

    #endregion
}