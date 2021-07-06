using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Artemis.Core;
using JetBrains.Profiler.SelfApi;

namespace Artemis.Plugins.Profiling.Prerequisites
{
    public class DotTracePrerequisite : PluginPrerequisite
    {
        private readonly Plugin _plugin;

        public DotTracePrerequisite(Plugin plugin)
        {
            _plugin = plugin;

            InstallActions = new List<PluginPrerequisiteAction>
            {
                new InstallDotTraceAction(_plugin.ResolveRelativePath(CpuProfiler.ProfilerDirectory))
            };
            UninstallActions = new List<PluginPrerequisiteAction>
            {
                new DeleteFolderAction("DotTrace", Path.Combine(_plugin.ResolveRelativePath(CpuProfiler.ProfilerDirectory), CpuProfiler.SubDirectory))
            };
        }

        #region Overrides of PluginPrerequisite

        public override bool IsMet()
        {
            return Directory.Exists(Path.Combine(_plugin.ResolveRelativePath(CpuProfiler.ProfilerDirectory), CpuProfiler.SubDirectory));
        }

        public override string Name => "DotTrace profiler";

        public override string Description => "The CPU profiler application by JetBrains";

        /// <inheritdoc />
        public override List<PluginPrerequisiteAction> InstallActions { get; }

        /// <inheritdoc />
        public override List<PluginPrerequisiteAction> UninstallActions { get; }

        #endregion
    }

    public class InstallDotTraceAction : PluginPrerequisiteAction, IProgress<double>
    {
        private readonly string _path;

        public InstallDotTraceAction(string path) : base("Install DotTrace")
        {
            _path = path;
        }

        public override async Task Execute(CancellationToken cancellationToken)
        {
            ShowProgressBar = true;
            await DotTrace.EnsurePrerequisiteAsync(cancellationToken, this, null, NuGetApi.V3, _path);
            Progress.Report((100, 100));
        }

        public void Report(double value)
        {
            Progress.Report(((long) value, 100));
        }
    }
}