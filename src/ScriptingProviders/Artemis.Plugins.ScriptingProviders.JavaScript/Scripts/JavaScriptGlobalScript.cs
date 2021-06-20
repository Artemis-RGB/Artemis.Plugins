using System;
using Artemis.Core.ScriptingProviders;
using Jint;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Scripts
{
    public class JavaScriptGlobalScript : GlobalScript
    {
        private Engine _engine;

        public JavaScriptGlobalScript(ScriptConfiguration configuration) : base(configuration)
        {
            _engine = new Engine(options =>
            {
                // Limit memory allocations to MB
                options.LimitMemory(4_000_000);

                // Set a timeout to 4 seconds.
                options.TimeoutInterval(TimeSpan.FromSeconds(4));

                // Set limit of 1000 executed statements.
                options.MaxStatements(1000);
            });

            _engine.Execute(ScriptConfiguration.ScriptContent);
            
        }
    }
}