using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Artemis.Core;
using JetBrains.Profiler.SelfApi;

namespace Artemis.Plugins.Profiling.Prerequisites
{
    public class DotMemoryPrerequisite : PluginPrerequisite
    {
        private readonly Plugin _plugin;

        public DotMemoryPrerequisite(Plugin plugin)
        {
            _plugin = plugin;

            InstallActions = new List<PluginPrerequisiteAction>
            {
                new InstallDotMemoryAction(_plugin.ResolveRelativePath(MemoryProfiler.ProfilerDirectory))
            };
            UninstallActions = new List<PluginPrerequisiteAction>
            {
                new DeleteFolderAction("DotMemory", Path.Combine(_plugin.ResolveRelativePath(MemoryProfiler.ProfilerDirectory), MemoryProfiler.SubDirectory))
            };
        }

        #region Overrides of PluginPrerequisite

        public override bool IsMet()
        {
            return Directory.Exists(Path.Combine(_plugin.ResolveRelativePath(MemoryProfiler.ProfilerDirectory), MemoryProfiler.SubDirectory));
        }

        public override string Name => "DotMemory profiler";

        public override string Description => "The memory profiler application by JetBrains";

        /// <inheritdoc />
        public override List<PluginPrerequisiteAction> InstallActions { get; }

        /// <inheritdoc />
        public override List<PluginPrerequisiteAction> UninstallActions { get; }

        #endregion
    }

    public class InstallDotMemoryAction : PluginPrerequisiteAction, IProgress<double>
    {
        private readonly string _path;

        public InstallDotMemoryAction(string path) : base("Install DotMemory")
        {
            _path = path;
        }

        public override async Task Execute(CancellationToken cancellationToken)
        {
            ShowProgressBar = true;
            await DotMemory.EnsurePrerequisiteAsync(cancellationToken, this, null, NuGetApi.V3, _path);
            Progress.Report((100, 100));
        }

        public void Report(double value)
        {
            Progress.Report(((long) value, 100));
        }
    }
}