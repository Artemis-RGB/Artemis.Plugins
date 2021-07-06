// This is your profile script. 
// - Hover over function names to learn about their functionality.
// - You can remove the callbacks below if you don't need them.
// - For a full guide check out https://wiki.artemis-rgb.com/guides/user/profiles/scripting

// The full profile is available via the GetProfile() function
const profile = Profile.GetProfile();

Profile.OnUpdating((deltaTime) => {
    // This is called each time the profile is about to update
});

Profile.OnUpdated((deltaTime) => {
    // This is called each time the profile has updated
});

Profile.OnRendering((canvas, bounds) => {
    // This is called when the profile is about to render
    // Here you can modify the canvas before the profile is drawn on top of it
});

Profile.OnRendererd((canvas, bounds) => {
    // This is called when the profile has rendered
    // Here you can modify the canvas after the profile is drawn
});