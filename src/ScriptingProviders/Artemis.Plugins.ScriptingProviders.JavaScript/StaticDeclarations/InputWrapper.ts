
declare class InputWrapper {
    /**
    * Register a new callback for whenever a key on a keyboard was pressed
    * @param callback The callback to call
    * @return A function that can be used to unsubscribe
    */
    OnKeyDown(callback: (key: Artemis.Core.KeyboardKey) => void): Function;

    /**
    * Register a new callback for whenever a key on a keyboard was released
    * @param callback The callback to call
    * @return A function that can be used to unsubscribe
    */
    OnKeyUp(callback: (key: Artemis.Core.KeyboardKey) => void): Function;

    /**
    * Register a new callback for whenever a button on a mouse was pressed
    * @param callback The callback to call
    * @return A function that can be used to unsubscribe
    */
    OnMouseDown(callback: (button: Artemis.Core.MouseButton) => void): Function;

    /**
    * Register a new callback for whenever a button on a mouse was released
    * @param callback The callback to call
    * @return A function that can be used to unsubscribe
    */
    OnMouseUp(callback: (button: Artemis.Core.MouseButton) => void): Function;
    
    /**
     *  Determines whether the provided key is pressed by any device
     */
    IsKeyDown(key: Artemis.Core.KeyboardKey): boolean;

    /**
     * Determines whether the button key is pressed by any device
     */
    IsButtonDown(button: Artemis.Core.MouseButton): boolean;
}

const Input = new InputWrapper();