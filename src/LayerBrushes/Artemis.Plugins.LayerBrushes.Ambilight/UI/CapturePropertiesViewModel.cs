using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        public CapturePropertiesViewModel(BaseLayerBrush layerBrush, IModelValidator<CapturePropertiesViewModel> validator) : base(layerBrush, validator)
        {
            Properties = ((AmbilightLayerBrush) layerBrush).Properties.Capture;
        }

        #endregion

        #region Properties & Fields

        public int X
        {
            get => Properties.X.CurrentValue;
            set => Properties.X.CurrentValue = value;
        }

        public int Y
        {
            get => Properties.Y.CurrentValue;
            set => Properties.Y.CurrentValue = value;
        }

        public int Width
        {
            get => Properties.Width.CurrentValue;
            set => Properties.Width.CurrentValue = value;
        }

        public int Height
        {
            get => Properties.Height.CurrentValue;
            set => Properties.Height.CurrentValue = value;
        }

        private IScreenCaptureService _screenCaptureService => AmbilightBootstrapper.ScreenCaptureService;

        private readonly Timer _displayPreviewTimer = new(500) {AutoReset = true};
        private readonly Timer _selectionPreviewTimer = new(33) {AutoReset = true};

        private bool _preventPreviewCreation;

        public AmbilightCaptureProperties Properties { get; }
        public List<DisplayPreview> Displays { get; private set; }

        private DisplayPreview _selectedDisplay;

        public DisplayPreview SelectedDisplay
        {
            get => _selectedDisplay;
            set
            {
                if (!SetAndNotify(ref _selectedDisplay, value)) return;
                if (Properties.X + Properties.Width > (value?.Display.Width ?? 0) ||
                    Properties.Y + Properties.Height > (value?.Display.Height ?? 0) ||
                    Properties.Width == 0 || Properties.Height == 0)
                {
                    _preventPreviewCreation = true;
                    Properties.X.BaseValue = 0;
                    Properties.Y.BaseValue = 0;
                    Properties.Width.BaseValue = value?.Display.Width ?? 0;
                    Properties.Height.BaseValue = value?.Display.Height ?? 0;
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
            Properties.X.BaseValue = 0;
            Properties.Y.BaseValue = 0;
            Properties.Width.BaseValue = SelectedDisplay?.Display.Width ?? 0;
            Properties.Height.BaseValue = SelectedDisplay?.Display.Height ?? 0;
        }

        public void RegionSelectMouseDown(object sender, MouseEventArgs args)
        {
            Canvas canvas = VisualTreeUtilities.FindParent<Canvas>((DependencyObject) sender, null);
            Point position = args.GetPosition(canvas);
            ((IInputElement) sender).CaptureMouse();
            _preventPreviewCreation = true;

            // Horizontal dragging
            if (Math.Abs(position.X - Properties.X) < 60)
                _dragLeft = true;
            else if (Math.Abs(position.X - (Properties.X + Properties.Width)) < 60)
                _dragRight = true;
            // Vertical dragging
            if (Math.Abs(position.Y - Properties.Y) < 60)
                _dragTop = true;
            else if (Math.Abs(position.Y - (Properties.Y + Properties.Height)) < 60)
                _dragBottom = true;
            // Movement dragging
            if (!_dragLeft && !_dragRight && !_dragLeft && !_dragTop && !_dragBottom)
            {
                _dragCenter = true;
                _centerDragOffset = new Point(Properties.X, Properties.Y) - position;
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
            if (args.LeftButton == MouseButtonState.Released) return;

            Canvas canvas = VisualTreeUtilities.FindParent<Canvas>((DependencyObject) sender, null);
            Point position = args.GetPosition(canvas);

            SKRectI region = SKRectI.Create(Properties.X, Properties.Y, Properties.Width, Properties.Height);

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

            Properties.X.BaseValue = Math.Max(0, region.Left);
            Properties.Y.BaseValue = Math.Max(0, region.Top);
            Properties.Width.BaseValue = Math.Max(1, region.Width);
            Properties.Height.BaseValue = Math.Max(1, region.Height);
        }

        private void RecreatePreview()
        {
            if (_preventPreviewCreation) return;


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

        public override Task<bool> CanCloseAsync() => ValidateAsync();

        protected override void OnInitialActivate()
        {
            base.OnInitialActivate();

            ((AmbilightLayerBrush) LayerBrush).PropertiesOpen = true;

            Displays = _screenCaptureService.GetGraphicsCards()
                .SelectMany(gg => _screenCaptureService.GetDisplays(gg))
                .Select(d => new DisplayPreview(d))
                .ToList();

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

        private void PropertiesOnLayerPropertyOnCurrentValueSet(object? sender, LayerPropertyEventArgs e)
        {
            NotifyOfPropertyChange(e.LayerProperty.Path.Split('.').Last());
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