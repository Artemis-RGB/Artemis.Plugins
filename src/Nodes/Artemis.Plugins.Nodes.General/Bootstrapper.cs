using Artemis.Core;
using DryIoc;
using Microsoft.Extensions.ObjectPool;
using NoStringEvaluating;
using NoStringEvaluating.Contract;
using NoStringEvaluating.Models.Values;
using NoStringEvaluating.Services.Cache;
using NoStringEvaluating.Services.Checking;
using NoStringEvaluating.Services.Parsing;
using NoStringEvaluating.Services.Parsing.NodeReaders;

namespace Artemis.Plugins.Nodes.General
{
    // This is your bootstrapper, you can do some kind of global setup work you need done here.
    // You can also just remove this file if you don't need it.
    public class Bootstrapper : PluginBootstrapper
    {
        public override void OnPluginEnabled(Plugin plugin)
        {
            // Pooling
            plugin.Container.RegisterInstance(ObjectPool.Create<Stack<InternalEvaluatorValue>>());
            plugin.Container.RegisterInstance(ObjectPool.Create<List<InternalEvaluatorValue>>());
            plugin.Container.RegisterInstance(ObjectPool.Create<ValueKeeperContainer>());
        
            // Parser
            plugin.Container.Register<IFormulaCache, FormulaCache>(Reuse.Singleton);
            plugin.Container.Register<IFunctionReader, FunctionReader>(Reuse.Singleton);
            plugin.Container.Register<IFormulaParser, FormulaParser>(Reuse.Singleton);
        
            // Checker
            plugin.Container.Register<IFormulaChecker, FormulaChecker>(Reuse.Singleton);
        
            // Evaluator
            plugin.Container.Register<INoStringEvaluator, NoStringEvaluator>(Reuse.Singleton);
            plugin.Container.Register<INoStringEvaluatorNullable, NoStringEvaluatorNullable>(Reuse.Singleton);
        }
    }
}