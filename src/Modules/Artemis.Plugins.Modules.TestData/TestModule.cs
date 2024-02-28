using System;
using System.Collections.Generic;
using Artemis.Core;
using Artemis.Core.Modules;
using Artemis.Core.Services;
using Artemis.Plugins.Modules.TestData.DataModels;
using SkiaSharp;

namespace Artemis.Plugins.Modules.TestData;

[PluginFeature(AlwaysEnabled = true)]
public class TestModule : Module<PluginDataModel>
{
    private readonly IWebServerService _webServerService;
    private Random _rand;

    public TestModule(IWebServerService webServerService)
    {
        _webServerService = webServerService;
    }

    public override List<IModuleActivationRequirement> ActivationRequirements => null;

    public override void Enable()
    {
        _rand = new Random();
        _webServerService.AddStringEndPoint(this, "StringEndPoint", s => DataModel.JsonString = s);
        _webServerService.AddResponsiveStringEndPoint(this, "StringEndPointWithResponse", s =>
        {
            DataModel.JsonString2 = s;
            return "la lu lo";
        });
        JsonPluginEndPoint<RemoteData> jsonPluginEndPoint = _webServerService.AddJsonEndPoint<RemoteData>(this, "JsonEndPoint", d => DataModel.JsonData = d);
        jsonPluginEndPoint.RequestException += JsonPluginEndPointOnRequestException;
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

    private void JsonPluginEndPointOnRequestException(object sender, EndpointExceptionEventArgs e)
    {
        throw e.Exception;
    }

    private void TimedUpdate(double deltaTime)
    {
        DataModel.TestColorA = SKColor.FromHsv(_rand.Next(0, 360), 100, 100);
        DataModel.TestColorB = SKColor.FromHsv(_rand.Next(0, 360), 100, 100);
    }
}