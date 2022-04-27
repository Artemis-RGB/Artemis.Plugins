using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using Artemis.Core.LayerBrushes;
using Artemis.Plugins.LayerBrushes.Ambilight.PropertyGroups;
using Artemis.UI.Shared.Extensions;
using Artemis.UI.Shared.LayerBrushes;
using DynamicData;
using ReactiveUI;
using ScreenCapture.NET;

namespace Artemis.Plugins.LayerBrushes.Ambilight.Screens
{
    public class CapturePropertiesViewModel : BrushConfigurationViewModel
    {
        private CaptureScreenViewModel? _selectedCaptureScreen;
        private ObservableAsPropertyHelper<int>? _x;
        private ObservableAsPropertyHelper<int>? _y;
        private ObservableAsPropertyHelper<int>? _width;
        private ObservableAsPropertyHelper<int>? _height;
        private IScreenCaptureService _screenCaptureService => AmbilightBootstrapper.ScreenCaptureService;

        public CapturePropertiesViewModel(BaseLayerBrush layerBrush) : base(layerBrush)
        {
            Properties = ((AmbilightLayerBrush) layerBrush).Properties.Capture;
            CaptureScreens = new ObservableCollection<CaptureScreenViewModel>();
            RegionEditor = new RegionEditorViewModel();
            RegionDisplay = new RegionDisplayViewModel();
            ResetRegion = ReactiveCommand.Create(ExecuteResetRegion);
            this.WhenActivated(d =>
            {
                CreateCaptureScreens();
                _x = Properties.X.AsObservable().ToProperty(this, vm => vm.X).DisposeWith(d);
                _y = Properties.X.AsObservable().ToProperty(this, vm => vm.Y).DisposeWith(d);
                _width = Properties.X.AsObservable().ToProperty(this, vm => vm.Width).DisposeWith(d);
                _height = Properties.X.AsObservable().ToProperty(this, vm => vm.Height).DisposeWith(d);

                Disposable.Create(() => CaptureScreens.Clear()).DisposeWith(d);
            });

            // this.ValidationRule(vm => vm.X, x => x < 0, "X needs to be 0 or greater");
            // this.ValidationRule(vm => vm.Y, x => x < 0, "Y needs to be 0 or greater");
            //
            // RuleFor(vm => vm.X).GreaterThanOrEqualTo(0).WithName("X").WithMessage("X needs to be 0 or greater");
            // RuleFor(vm => vm.Y).GreaterThanOrEqualTo(0).WithName("Y").WithMessage("Y needs to be 0 or greater");
            // RuleFor(vm => vm.Width).GreaterThan(0).WithName("Width").WithMessage("Width needs to be greater than 0");
            // RuleFor(vm => vm.Height).GreaterThan(0).WithName("Height").WithMessage("Height needs to be greater than 0");
            // RuleFor(vm => vm.SelectedDisplay).NotNull().WithName("Display").WithMessage("No display selected.").DependentRules(() =>
            // {
            //     RuleFor(vm => vm.X + vm.Width).LessThanOrEqualTo(vm => vm.SelectedDisplay.Display.Width).WithName("X/Width").WithMessage("The region exceeds the display width.");
            //     RuleFor(vm => vm.Y + vm.Height).LessThanOrEqualTo(vm => vm.SelectedDisplay.Display.Height).WithName("Y/Height").WithMessage("The region exceeds the display height.");
            // });
        }


        public AmbilightCaptureProperties Properties { get; }
        public ObservableCollection<CaptureScreenViewModel> CaptureScreens { get; }
        public RegionEditorViewModel RegionEditor { get; }
        public RegionDisplayViewModel RegionDisplay { get; }

        public ReactiveCommand<Unit, Unit> ResetRegion { get; }

        public CaptureScreenViewModel? SelectedCaptureScreen
        {
            get => _selectedCaptureScreen;
            set => this.RaiseAndSetIfChanged(ref _selectedCaptureScreen, value);
        }

        public int X
        {
            get => _x?.Value ?? 0;
            set => Properties.X.SetCurrentValue(value);
        }

        public int Y
        {
            get => _y?.Value ?? 0;
            set => Properties.Y.SetCurrentValue(value);
        }

        public int Width
        {
            get => _width?.Value ?? 0;
            set => Properties.Width.SetCurrentValue(value);
        }

        public int Height
        {
            get => _height?.Value ?? 0;
            set => Properties.Height.SetCurrentValue(value);
        }

        private void CreateCaptureScreens()
        {
            CaptureScreens.AddRange(_screenCaptureService.GetGraphicsCards()
                .SelectMany(gg => _screenCaptureService.GetDisplays(gg))
                .Select(d => new CaptureScreenViewModel(d))
                .ToList());
        }

        private void Update()
        {
            foreach (CaptureScreenViewModel captureScreenViewModel in CaptureScreens)
                captureScreenViewModel.Update();
        }

        private void ExecuteResetRegion()
        {
        }
    }
}