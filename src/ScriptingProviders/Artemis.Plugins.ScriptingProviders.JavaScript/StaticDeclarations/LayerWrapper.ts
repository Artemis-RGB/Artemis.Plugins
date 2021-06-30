
declare class LayerWrapper {
    /**
    * Register a new callback for whenever the layer is about to update
    * @param callback The callback to call
    * @return A function that can be used to unsubscribe
    */
    OnUpdating(callback: (deltaTime: number) => void): Function;

    /**
    * Register a new callback for whenever the layer has updated
    * @param callback The callback to call
    * @return A function that can be used to unsubscribe
    */
    OnUpdated(callback: (deltaTime: number) => void): Function;

    /**
    * Register a new callback for whenever the layer is about to render
    * @param callback The callback to call
    * @return A function that can be used to unsubscribe
    */
    OnRendering(callback: (canvas: SkiaSharp.SKCanvas, bounds: SkiaSharp.SKRect, paint: SkiaSharp.SKPaint) => void): Function;

    /**
    * Register a new callback for whenever the layer has rendered
    * @param callback The callback to call
    * @return A function that can be used to unsubscribe
    */
    OnRendered(callback: (canvas: SkiaSharp.SKCanvas, bounds: SkiaSharp.SKRect, paint: SkiaSharp.SKPaint) => void): Function;
    
    /**
     * Returns the actual layer
     */
    GetLayer(): Artemis.Core.Layer;
}

const Layer = new LayerWrapper();