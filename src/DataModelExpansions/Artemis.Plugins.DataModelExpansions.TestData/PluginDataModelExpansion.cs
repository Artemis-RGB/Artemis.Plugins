using System;
using Artemis.Core;
using Artemis.Core.DataModelExpansions;
using Artemis.Plugins.DataModelExpansions.TestData.DataModels;
using Artemis.Plugins.DataModelExpansions.TestData.ViewModels;
using SkiaSharp;

namespace Artemis.Plugins.DataModelExpansions.TestData
{
    public class PluginDataModelExpansion : DataModelExpansion<PluginDataModel>
    {
        private Random _rand;

        public override void EnablePlugin()
        {
            _rand = new Random();
            ConfigurationDialog = new PluginConfigurationDialog<TestPluginConfigurationViewModel>();
        }

        public override void DisablePlugin()
        {
        }

        public override void Update(double deltaTime)
        {
            // You can access your data model here and update it however you like
            DataModel.TemplateDataModelString = $"The last delta time was {deltaTime} seconds";
        }

        private void TimedUpdate(double deltaTime)
        {
            DataModel.TestColorA = SKColor.FromHsv(_rand.Next(0, 360), 100, 100);
            DataModel.TestColorB = SKColor.FromHsv(_rand.Next(0, 360), 100, 100);
        }
    }
}