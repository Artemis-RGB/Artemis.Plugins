using System;
using System.IO;
using System.Threading.Tasks;
using Artemis.Core;
using Artemis.Core.Services;
using Artemis.UI.Shared;
using Artemis.UI.Shared.Services;
using MaterialDesignThemes.Wpf;
using Ookii.Dialogs.Wpf;

namespace Artemis.Plugins.Profiling.ViewModels
{
    public class ProfilerConfigurationViewModel : PluginConfigurationViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly IPluginManagementService _pluginManagementService;
        private CpuProfiler _cpuProfiler;
        private bool _isCpuBusy;
        private bool _isMemoryBusy;

        private bool _isProfilingCpu;
        private bool _isProfilingMemory;
        private MemoryProfiler _memoryProfiler;

        public ProfilerConfigurationViewModel(Plugin plugin, IPluginManagementService pluginManagementService, IDialogService dialogService) : base(plugin)
        {
            _pluginManagementService = pluginManagementService;
            _dialogService = dialogService;
            ProfilingMessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));
        }

        public SnackbarMessageQueue ProfilingMessageQueue { get; }

        public bool IsCpuProfilerAvailable => _cpuProfiler != null;
        public bool IsMemoryProfilerAvailable => _memoryProfiler != null;

        public bool IsCpuBusy
        {
            get => _isCpuBusy;
            set => SetAndNotify(ref _isCpuBusy, value);
        }

        public bool IsMemoryBusy
        {
            get => _isMemoryBusy;
            set => SetAndNotify(ref _isMemoryBusy, value);
        }

        public bool IsProfilingCpu
        {
            get => _isProfilingCpu;
            set => SetAndNotify(ref _isProfilingCpu, value);
        }

        public bool IsProfilingMemory
        {
            get => _isProfilingMemory;
            set => SetAndNotify(ref _isProfilingMemory, value);
        }

        public void StartCpuProfiling()
        {
            if (_cpuProfiler == null || IsCpuBusy)
                return;

            VistaFolderBrowserDialog dialog = new() {Description = "Profile result directory"};
            bool? result = dialog.ShowDialog();
            if (result is not true)
                return;

            IsCpuBusy = true;
            if (!Directory.Exists(dialog.SelectedPath))
                Directory.CreateDirectory(dialog.SelectedPath);

            Task.Run(() =>
            {
                try
                {
                    _cpuProfiler.StartProfiling(dialog.SelectedPath);
                    IsProfilingCpu = _cpuProfiler.Profiling;
                    IsCpuBusy = false;

                    ProfilingMessageQueue.Enqueue("Started CPU profiling");
                }
                catch (Exception e)
                {
                    _dialogService.ShowExceptionDialog("An error occurred while starting CPU profiling", e);
                }
            });
        }

        public void TakeCpuSnapshot()
        {
            if (_cpuProfiler == null || !IsProfilingCpu)
                return;

            _cpuProfiler.TakeSnapshot();
            ProfilingMessageQueue.Enqueue("Took CPU snapshot");
        }

        public void StopCpuProfiling()
        {
            if (_cpuProfiler == null || IsCpuBusy)
                return;

            Task.Run(() =>
            {
                try
                {
                    IsCpuBusy = true;
                    _cpuProfiler.StopProfiling();
                    IsProfilingCpu = _cpuProfiler.Profiling;
                    IsCpuBusy = false;

                    ProfilingMessageQueue.Enqueue("Finished CPU profiling");
                }
                catch (Exception e)
                {
                    _dialogService.ShowExceptionDialog("An error occurred while stopping CPU profiling", e);
                }
            });
        }

        public void StartMemoryProfiling()
        {
            if (_memoryProfiler == null || IsMemoryBusy)
                return;

            VistaFolderBrowserDialog dialog = new() {Description = "Profile result directory"};
            bool? result = dialog.ShowDialog();
            if (result is not true)
                return;

            IsMemoryBusy = true;
            if (!Directory.Exists(dialog.SelectedPath))
                Directory.CreateDirectory(dialog.SelectedPath);

            Task.Run(() =>
            {
                try
                {
                    _memoryProfiler.StartProfiling(dialog.SelectedPath);
                    IsProfilingMemory = _memoryProfiler.Profiling;
                    IsMemoryBusy = false;

                    ProfilingMessageQueue.Enqueue("Started memory profiling");
                }
                catch (Exception e)
                {
                    _dialogService.ShowExceptionDialog("An error occurred while starting memory profiling", e);
                }
            });
        }

        public void TakeMemorySnapshot()
        {
            if (_memoryProfiler == null || !IsProfilingMemory)
                return;

            _memoryProfiler.TakeSnapshot();
            ProfilingMessageQueue.Enqueue("Took memory snapshot");
        }

        public void StopMemoryProfiling()
        {
            if (_memoryProfiler == null || IsMemoryBusy)
                return;

            Task.Run(() =>
            {
                try
                {
                    IsMemoryBusy = true;
                    _memoryProfiler.StopProfiling();
                    IsProfilingMemory = _memoryProfiler.Profiling;
                    IsMemoryBusy = false;

                    ProfilingMessageQueue.Enqueue("Finished memory profiling");
                }
                catch (Exception e)
                {
                    _dialogService.ShowExceptionDialog("An error occurred while stopping memory profiling", e);
                }
            });
        }

        protected override void OnInitialActivate()
        {
            _pluginManagementService.PluginFeatureEnabled += PluginManagementServiceOnPluginFeatureEnabled;
            _pluginManagementService.PluginFeatureDisabled += PluginManagementServiceOnPluginFeatureEnabled;
            GetProfilers();

            base.OnInitialActivate();
        }

        protected override void OnClose()
        {
            _pluginManagementService.PluginFeatureEnabled -= PluginManagementServiceOnPluginFeatureEnabled;
            _pluginManagementService.PluginFeatureDisabled -= PluginManagementServiceOnPluginFeatureEnabled;
            base.OnClose();
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

            NotifyOfPropertyChange(nameof(IsCpuProfilerAvailable));
            NotifyOfPropertyChange(nameof(IsMemoryProfilerAvailable));
        }
    }
}