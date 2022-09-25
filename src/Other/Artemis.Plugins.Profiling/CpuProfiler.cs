using Artemis.Core;
using JetBrains.Profiler.SelfApi;

namespace Artemis.Plugins.Profiling
{
    [PluginFeature(Name = "CPU Profiler")]
    public class CpuProfiler : PluginFeature
    {
        public static string ProfilerDirectory = "JetBrains";
        public static string SubDirectory = "dotTrace";

        private readonly Plugin _plugin;

        public CpuProfiler(Plugin plugin)
        {
            _plugin = plugin;
        }

        public bool Profiling { get; private set; }

        public override void Enable()
        {
        }

        public override void Disable()
        {
            CancelProfiling();
        }

        public void StartProfiling(string dirPath)
        {
            lock (this)
            {
                if (Profiling)
                    return;

                DotTrace.EnsurePrerequisite(null, NuGetApi.V3, _plugin.ResolveRelativePath(ProfilerDirectory));

                DotTrace.Config config = new();
                config.UseTimelineProfilingType();
                config.SaveToDir(dirPath);

                DotTrace.Attach(config);
                DotTrace.StartCollectingData();

                Profiling = true;
            }
        }

        public void TakeSnapshot()
        {
            lock (this)
            {
                if (!Profiling)
                    return;

                DotTrace.SaveData();
                DotTrace.StartCollectingData();
            }
        }

        public void CancelProfiling()
        {
            lock (this)
            {
                if (!Profiling)
                    return;

                DotTrace.DropData();
                DotTrace.Detach();

                Profiling = false;
            }
        }

        public void StopProfiling()
        {
            lock (this)
            {
                if (!Profiling)
                    return;

                DotTrace.SaveData();
                DotTrace.Detach();
                DotTrace.GetCollectedSnapshotFilesArchive(true);

                Profiling = false;
            }
        }
    }
}