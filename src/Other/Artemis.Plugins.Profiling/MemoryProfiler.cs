using Artemis.Core;
using JetBrains.Profiler.SelfApi;

namespace Artemis.Plugins.Profiling
{
    [PluginFeature(Icon = "Ruler")]
    public class MemoryProfiler : PluginFeature
    {
        public static string ProfilerDirectory = "JetBrains";
        public static string SubDirectory = "dotMemory";
        private readonly Plugin _plugin;

        public MemoryProfiler(Plugin plugin)
        {
            _plugin = plugin;
        }

        public bool Profiling { get; private set; }

        public override void Enable()
        {
        }

        public override void Disable()
        {
            StopProfiling();
        }

        public void StartProfiling(string dirPath)
        {
            lock (this)
            {
                if (Profiling)
                    return;

                DotMemory.EnsurePrerequisite(null, NuGetApi.V3, _plugin.ResolveRelativePath(ProfilerDirectory));

                DotMemory.Config config = new();
                config.SaveToDir(dirPath);
                
                DotMemory.Attach(config);
                Profiling = true;
            }
        }

        public void TakeSnapshot()
        {
            lock (this)
            {
                if (!Profiling)
                    return;

                DotMemory.GetSnapshot();
            }
        }

        public void StopProfiling()
        {
            lock (this)
            {
                if (!Profiling)
                    return;

                string workspacePath = DotMemory.Detach();

                Profiling = false;
            }
        }
    }
}