
declare class ProfileWrapper {
    /**
    * Register a new callback for whenever the profile is about to update
    * @param callback The callback to call
    * @return A function that can be used to unsubscribe
    */
    OnUpdating(callback: (deltaTime: number) => void): Function;

    /**
    * Register a new callback for whenever the profile has updated
    * @param callback The callback to call
    * @return A function that can be used to unsubscribe
    */
    OnUpdated(callback: (deltaTime: number) => void): Function;

    /**
    * Register a new callback for whenever the profile is about to render
    * @param callback The callback to call
    * @return A function that can be used to unsubscribe
    */
    OnRendering(callback: (canvas: SkiaSharp.SKCanvas, bounds: SkiaSharp.SKRect) => void): Function;

    /**
    * Register a new callback for whenever the profile has rendered
    * @param callback The callback to call
    * @return A function that can be used to unsubscribe
    */
    OnRendered(callback: (canvas: SkiaSharp.SKCanvas, bounds: SkiaSharp.SKRect) => void): Function;
    
    /**
     * Returns the actual profile
     */
    GetProfile(): Artemis.Core.Profile;

    /**
     * Returns an array containing all folders of the profile
     */
    GetFolders(): Artemis.Core.Folder[];

    /**
     * Returns an array containing all layers of the profile
     */
    GetLayers(): Artemis.Core.Layer[];
}

const Profile = new ProfileWrapper();