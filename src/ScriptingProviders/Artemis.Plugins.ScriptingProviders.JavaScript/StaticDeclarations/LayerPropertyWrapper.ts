declare class LayerPropertyWrapper {
    /**
     * Gets the description of this layer property
     */

    readonly Description: Artemis.Core.PropertyDescriptionAttribute;
    /**
     * Gets the C# type of this layer property
     */

    readonly PropertyType: string;
    /**
     * Gets the unique path of this property on the layer
     */
    readonly Path: string;

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
}

const LayerProperty = new LayerPropertyWrapper();