using System;
using System.Reactive.Disposables;
using Artemis.Plugins.PhilipsHue.ViewModels;
using Artemis.Plugins.PhilipsHue.Views.Steps;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Navigation;
using ReactiveUI;

namespace Artemis.Plugins.PhilipsHue.Views;

public class PhilipsHueConfigurationView : ReactiveUserControl<PhilipsHueConfigurationViewModel>
{
    private readonly Frame _frame;

    public PhilipsHueConfigurationView()
    {
        InitializeComponent();
        _frame = this.Get<Frame>("Frame");

        this.WhenActivated(d => ViewModel.WhenAnyValue(vm => vm.WizardPage).Subscribe(ApplyWizardPage).DisposeWith(d));
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void ApplyWizardPage(int page)
    {
        switch (page)
        {
            case 0:
                _frame.NavigateToType(typeof(WelcomeStep), null, new FrameNavigationOptions());
                break;
            case 1:
                _frame.NavigateToType(typeof(SyncStep), null, new FrameNavigationOptions());
                break;
            case 2:
                _frame.NavigateToType(typeof(FinishStep), null, new FrameNavigationOptions());
                break;
            case 3:
                _frame.NavigateToType(typeof(SettingsStep), null, new FrameNavigationOptions());
                break;
        }
    }
}