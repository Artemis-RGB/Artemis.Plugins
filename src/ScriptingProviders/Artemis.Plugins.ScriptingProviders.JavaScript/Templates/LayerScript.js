// This is your layer script. 
// - Hover over function names to learn about their functionality.
// - You can remove the callbacks below if you don't need them.
// - For a full guide check out https://wiki.artemis-rgb.com/guides/user/layers/scripting

// The full layer is available via the GetLayer() function
const layer = Layer.GetLayer();

Layer.OnUpdating((deltaTime) => {
    // This is called each time the layer is about to update
});

Layer.OnUpdated((deltaTime) => {
    // This is called each time the layer has updated
});

Layer.OnRendering((canvas, bounds, paint) => {
    // This is called when the layer is about to render
    // Here you can modify the canvas before the layer is drawn on top of it
});

Layer.OnRendering((canvas, bounds, paint) => {
    // This is called when the layer has rendered
    // Here you can modify the canvas after the layer is drawn
});