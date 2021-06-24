
declare class Profile {
    /**
    * Register a new callback for whenever the profile is about to update
    * @param callback The callback to call
    * @return A function that can be used to unsubscribe
    */
    onUpdating(callback: (deltaTime: number) => void): Function;

    /**
    * Register a new callback for whenever the profile has updated
    * @param callback The callback to call
    * @return A function that can be used to unsubscribe
    */
    onUpdated(callback: (deltaTime: number) => void): Function;

    /**
    * Register a new callback for whenever the profile is about to render
    * @param callback The callback to call
    * @return A function that can be used to unsubscribe
    */
    onRendering(callback: (canvas: SkiaSharp.SKCanvas, bounds: SkiaSharp.SKRect) => void): Function;

    /**
    * Register a new callback for whenever the profile has rendered
    * @param callback The callback to call
    * @return A function that can be used to unsubscribe
    */
    onRendered(callback: (canvas: SkiaSharp.SKCanvas, bounds: SkiaSharp.SKRect) => void): Function;

    /**
     * Returns an array containing all folders of the profile
     */
    getFolders(): Artemis.Core.Folder[];

    /**
     * Returns an array containing all layers of the profile
     */
    getLayers(): Artemis.Core.Layer[];
}

const profile = new Profile();