using System;
using System.Collections.Generic;
using Artemis.Core.DataModelExpansions;
using Artemis.Core.Services;
using Artemis.Plugins.DataModelExpansions.TestData.DataModels;
using SkiaSharp;

namespace Artemis.Plugins.DataModelExpansions.TestData
{
    public class PluginDataModelExpansion : DataModelExpansion<PluginDataModel>
    {
        private readonly IWebServerService _webServerService;

        public PluginDataModelExpansion(IWebServerService webServerService)
        {
            _webServerService = webServerService;
        }

        private Random _rand;

        public override void Enable()
        {
            _rand = new Random();
            _webServerService.AddStringEndPoint(this, "StringEndPoint", s => DataModel.JsonString = s);
            _webServerService.AddResponsiveStringEndPoint(this, "StringEndPointWithResponse", s =>
            {
                DataModel.JsonString2 = s;
                return "la lu lo";
            });
            _webServerService.AddJsonEndPoint<RemoteData>(this, "JsonEndPoint", d => DataModel.JsonData = d);
            _webServerService.AddResponsiveJsonEndPoint<RemoteData>(this, "JsonEndPointWithResponse", d =>
            {
                DataModel.JsonData2 = d;
                return new List<string> {"la", "lu", "lo"};
            });
        }


        public override void Disable()
        {
        }

        public override void Update(double deltaTime)
        {
            // You can access your data model here and update it however you like
            DataModel.TemplateDataModelString = $"The last delta time was {deltaTime} seconds";

            DataModel.Rotation++;
            if (DataModel.Rotation > 360)
                DataModel.Rotation = 0;
        }

        private void TimedUpdate(double deltaTime)
        {
            DataModel.TestColorA = SKColor.FromHsv(_rand.Next(0, 360), 100, 100);
            DataModel.TestColorB = SKColor.FromHsv(_rand.Next(0, 360), 100, 100);
        }
    }
}