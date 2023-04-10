using System;
using System.Reactive.Disposables;
using Artemis.Plugins.PhilipsHue.ViewModels;
using Artemis.Plugins.PhilipsHue.Views.Steps;
using Avalonia.ReactiveUI;
using FluentAvalonia.UI.Navigation;
using ReactiveUI;

namespace Artemis.Plugins.PhilipsHue.Views;

public partial class PhilipsHueConfigurationView : ReactiveUserControl<PhilipsHueConfigurationViewModel>
{
    public PhilipsHueConfigurationView()
    {
        InitializeComponent();
        this.WhenActivated(d => ViewModel.WhenAnyValue(vm => vm.WizardPage).Subscribe(ApplyWizardPage).DisposeWith(d));
    }


    private void ApplyWizardPage(int page)
    {
        switch (page)
        {
            case 0:
                ConfigurationFrame.NavigateToType(typeof(WelcomeStep), null, new FrameNavigationOptions());
                break;
            case 1:
                ConfigurationFrame.NavigateToType(typeof(SyncStep), null, new FrameNavigationOptions());
                break;
            case 2:
                ConfigurationFrame.NavigateToType(typeof(FinishStep), null, new FrameNavigationOptions());
                break;
            case 3:
                ConfigurationFrame.NavigateToType(typeof(SettingsStep), null, new FrameNavigationOptions());
                break;
        }
    }
}