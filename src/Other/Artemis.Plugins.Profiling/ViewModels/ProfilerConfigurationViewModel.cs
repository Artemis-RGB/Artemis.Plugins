using System;
using System.IO;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Artemis.Core;
using Artemis.Core.Services;
using Artemis.UI.Shared;
using Artemis.UI.Shared.Services;
using Artemis.UI.Shared.Services.Builders;
using ReactiveUI;

namespace Artemis.Plugins.Profiling.ViewModels;

public class ProfilerConfigurationViewModel : PluginConfigurationViewModel
{
    private readonly INotificationService _notificationService;
    private readonly IWindowService _windowService;

    private CpuProfiler _cpuProfiler;
    private bool _isCpuBusy;
    private bool _isMemoryBusy;
    private bool _isProfilingCpu;
    private bool _isProfilingMemory;
    private MemoryProfiler _memoryProfiler;

    public ProfilerConfigurationViewModel(Plugin plugin, IPluginManagementService pluginManagementService, IWindowService windowService, INotificationService notificationService) : base(plugin)
    {
        _windowService = windowService;
        _notificationService = notificationService;

        StartCpuProfiling = ReactiveCommand.CreateFromTask(ExecuteStartCpuProfiling, this.WhenAnyValue(vm => vm.IsCpuProfilerAvailable));
        StopCpuProfiling = ReactiveCommand.CreateFromTask(ExecuteStopCpuProfiling, this.WhenAnyValue(vm => vm.IsProfilingCpu, vm => vm.IsCpuBusy, (p, b) => p && !b));
        TakeCpuSnapshot = ReactiveCommand.CreateFromTask(ExecuteTakeCpuSnapshot, this.WhenAnyValue(vm => vm.IsCpuBusy, b => !b));

        StartMemoryProfiling = ReactiveCommand.CreateFromTask(ExecuteStartMemoryProfiling, this.WhenAnyValue(vm => vm.IsMemoryProfilerAvailable));
        StopMemoryProfiling = ReactiveCommand.CreateFromTask(ExecuteStopMemoryProfiling, this.WhenAnyValue(vm => vm.IsProfilingMemory, vm => vm.IsMemoryBusy, (p, b) => p && !b));
        TakeMemorySnapshot = ReactiveCommand.CreateFromTask(ExecuteTakeMemorySnapshot, this.WhenAnyValue(vm => vm.IsMemoryBusy, b => !b));

        this.WhenActivated(d =>
        {
            pluginManagementService.PluginFeatureEnabled += PluginManagementServiceOnPluginFeatureEnabled;
            pluginManagementService.PluginFeatureDisabled += PluginManagementServiceOnPluginFeatureEnabled;
            GetProfilers();

            Disposable.Create(() =>
            {
                pluginManagementService.PluginFeatureEnabled -= PluginManagementServiceOnPluginFeatureEnabled;
                pluginManagementService.PluginFeatureDisabled -= PluginManagementServiceOnPluginFeatureEnabled;
            }).DisposeWith(d);
        });
    }

    public ReactiveCommand<Unit, Unit> StartCpuProfiling { get; }
    public ReactiveCommand<Unit, Unit> StopCpuProfiling { get; }
    public ReactiveCommand<Unit, Unit> TakeCpuSnapshot { get; }
    public ReactiveCommand<Unit, Unit> StartMemoryProfiling { get; }
    public ReactiveCommand<Unit, Unit> StopMemoryProfiling { get; }
    public ReactiveCommand<Unit, Unit> TakeMemorySnapshot { get; }

    public bool IsCpuProfilerAvailable => _cpuProfiler != null;
    public bool IsMemoryProfilerAvailable => _memoryProfiler != null;

    public bool IsCpuBusy
    {
        get => _isCpuBusy;
        set => RaiseAndSetIfChanged(ref _isCpuBusy, value);
    }

    public bool IsMemoryBusy
    {
        get => _isMemoryBusy;
        set => RaiseAndSetIfChanged(ref _isMemoryBusy, value);
    }

    public bool IsProfilingCpu
    {
        get => _isProfilingCpu;
        set => RaiseAndSetIfChanged(ref _isProfilingCpu, value);
    }

    public bool IsProfilingMemory
    {
        get => _isProfilingMemory;
        set => RaiseAndSetIfChanged(ref _isProfilingMemory, value);
    }

    private async Task ExecuteStartCpuProfiling()
    {
        if (_cpuProfiler == null || IsCpuBusy)
            return;

        string folder = await _windowService.CreateOpenFolderDialog().WithTitle("Profile result directory").ShowAsync();
        if (folder == null)
            return;

        IsCpuBusy = true;
        try
        {
            await Task.Run(() =>
            {
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                _cpuProfiler.StartProfiling(folder);
            });
            _notificationService.CreateNotification().WithMessage("Started CPU profiling").Show();
        }
        catch (Exception e)
        {
            _windowService.ShowExceptionDialog("An error occurred while starting CPU profiling", e);
        }
        finally
        {
            IsProfilingCpu = _cpuProfiler.Profiling;
            IsCpuBusy = false;
        }
    }

    private async Task ExecuteTakeCpuSnapshot()
    {
        IsCpuBusy = true;
        try
        {
            await Task.Run(() => _cpuProfiler.TakeSnapshot());
            _notificationService.CreateNotification().WithSeverity(NotificationSeverity.Success).WithMessage("Took CPU snapshot").Show();
        }
        catch (Exception e)
        {
            _windowService.ShowExceptionDialog("An error occurred while taking CPU snapshot", e);
        }
        finally
        {
            IsCpuBusy = false;
        }
    }

    private async Task ExecuteStopCpuProfiling()
    {
        if (_cpuProfiler == null || IsCpuBusy)
            return;

        IsCpuBusy = true;
        try
        {
            await Task.Run(() => _cpuProfiler.StopProfiling());
            _notificationService.CreateNotification().WithSeverity(NotificationSeverity.Success).WithMessage("Finished CPU profiling").Show();
        }
        catch (Exception e)
        {
            _windowService.ShowExceptionDialog("An error occurred while stopping CPU profiling", e);
        }
        finally
        {
            IsProfilingCpu = _cpuProfiler.Profiling;
            IsCpuBusy = false;
        }
    }

    private async Task ExecuteStartMemoryProfiling()
    {
        if (_memoryProfiler == null || IsMemoryBusy)
            return;

        string folder = await _windowService.CreateOpenFolderDialog().WithTitle("Profile result directory").ShowAsync();
        if (folder == null)
            return;

        IsMemoryBusy = true;
        try
        {
            await Task.Run(() =>
            {
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                _memoryProfiler.StartProfiling(folder);
            });
            _notificationService.CreateNotification().WithMessage("Started memory profiling").Show();
        }
        catch (Exception e)
        {
            _windowService.ShowExceptionDialog("An error occurred while starting memory profiling", e);
        }
        finally
        {
            IsProfilingMemory = _memoryProfiler.Profiling;
            IsMemoryBusy = false;
        }
    }

    private async Task ExecuteTakeMemorySnapshot()
    {
        if (_memoryProfiler == null || !IsProfilingMemory)
            return;

        IsMemoryBusy = true;
        try
        {
            await Task.Run(() => _memoryProfiler.TakeSnapshot());
            _notificationService.CreateNotification().WithSeverity(NotificationSeverity.Success).WithMessage("Took memory snapshot").Show();
        }
        catch (Exception e)
        {
            _windowService.ShowExceptionDialog("An error occurred while taking memory snapshot", e);
        }
        finally
        {
            IsMemoryBusy = false;
        }
    }

    private async Task ExecuteStopMemoryProfiling()
    {
        if (_memoryProfiler == null || IsMemoryBusy)
            return;

        IsMemoryBusy = true;
        try
        {
            await Task.Run(() => _memoryProfiler.StopProfiling());
            _notificationService.CreateNotification().WithSeverity(NotificationSeverity.Success).WithMessage("Finished memory profiling").Show();
        }
        catch (Exception e)
        {
            _windowService.ShowExceptionDialog("An error occurred while stopping memory profiling", e);
        }
        finally
        {
            IsProfilingMemory = _memoryProfiler.Profiling;
            IsMemoryBusy = false;
        }
    }

    private void PluginManagementServiceOnPluginFeatureEnabled(object sender, PluginFeatureEventArgs e)
    {
        GetProfilers();
    }

    private void GetProfilers()
    {
        if (Plugin.GetFeature<CpuProfiler>()?.IsEnabled == true)
        {
            _cpuProfiler = Plugin.GetFeature<CpuProfiler>();
            IsProfilingCpu = _cpuProfiler!.Profiling;
        }
        else
        {
            _cpuProfiler = null;
            IsProfilingCpu = false;
        }

        if (Plugin.GetFeature<MemoryProfiler>()?.IsEnabled == true)
        {
            _memoryProfiler = Plugin.GetFeature<MemoryProfiler>();
            IsProfilingMemory = _memoryProfiler!.Profiling;
        }
        else
        {
            _memoryProfiler = null;
            IsProfilingMemory = false;
        }

        this.RaisePropertyChanged(nameof(IsCpuProfilerAvailable));
        this.RaisePropertyChanged(nameof(IsMemoryProfilerAvailable));
    }
}