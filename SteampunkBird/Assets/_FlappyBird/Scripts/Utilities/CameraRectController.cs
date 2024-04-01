using UnityEngine;

/// <summary>
/// This script can be used to limit the size of the camera in your games.
/// It can be useful when you're trying to create a game that has portrait orientation
/// and run it on the PC. It will also automatically center the view.
/// This can be used because the resolution that we've set in our Game View is only applicable in Editor.
/// Built projects will use the full screen resolution no matter what is set in the Game View window.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraRectController : MonoBehaviour
{
    //You can also notice here that we can define a default values that will be set up when we add the component to the object.
    
    /// <summary>
    /// The Aspect Ratio we've designed our game for.
    /// </summary>
    public Vector2 AspectRatio = new(9, 16);

    /// <summary>
    /// The reference to the Camera component.
    /// </summary>
    public Camera gameCamera;

    /// <summary>
    /// This is the last resolution we've set our camera value for.
    /// It allows us to check if the resolution changed during the game
    /// e.g. player entered/exited fullscreen.
    /// </summary>
    private Vector2 _lastResolution;
    
    private void Start()
    {
        //We will do this once when the game starts for good measure
        SetCameraRectForSpecifiedAspectRatio();
    }

    private void Update()
    {
        //We can check if the resolution has changed, and only if it changed we will set the Camera Rect again. 
        //Because of the floating-point comparison errors we need to check if we're approximately those values instead of using != or ==.
        if(!Mathf.Approximately(_lastResolution.x, Screen.width) || !Mathf.Approximately(_lastResolution.y, Screen.height))
            SetCameraRectForSpecifiedAspectRatio();
    }

    public void SetCameraRectForSpecifiedAspectRatio()
    {
        //We can check the current resolution of the game.
        var currentResolution = new Vector2(Screen.width, Screen.height);

        var currentAspectRatio = currentResolution.x / currentResolution.y;
        var newAspectRatio = AspectRatio.x / AspectRatio.y;
        
        var scaledRectHeight = currentAspectRatio / newAspectRatio;

        //Because the _camera.rect is a struct we cannot edit the rect values directly as they are copies.
        //But we can save them to a variable, edit them in the variable and then assign them back to the camera.
        var cameraRect = gameCamera.rect;

        //If both new and current aspect ratio are similar, then just set the default values for the camera rect.
        if (Mathf.Approximately(newAspectRatio, currentAspectRatio))
        {
            cameraRect.height = 1;
            cameraRect.width = 1;
            cameraRect.x = 0;
            cameraRect.y = 0;
            
            //This will actually change the values in the Camera component.
            gameCamera.rect = cameraRect;
            
            return;
        }
        
        //We can set up something called Rect - which is a description of a rectangle that has width, height, x and y position - and define it for our camera.
        //The width and height are values that describe how big percentage of the game window (be it Game View in Editor, an actual window in packaged project or a 
        //resolution of our monitor when the game is in fullscreen). The x and y work the same in terms of position of it. 
        
        //We can define that our camera should occupy some part of the actual game window instead of taking up all available space.
        //This can be used if you want to have a split-screen for multiple player with many cameras or - in our case - limit our game to a 9:16 aspect ratio 
        //no matter the size of the window the player is running the game on.
        
        //If the scaledRectHeight is lesser than 1.0 this means that we should add the letterboxes to the top and bottom of the screen
        //Otherwise if it's greater than 1.0, we need to add pillarboxes to the sides of the screen.
        if (scaledRectHeight < 1.0f)
        {
            //We want our camera rect to be stretched fully in width and start at the leftmost side of the screen
            cameraRect.width = 1.0f;    
            cameraRect.x = 0;

            cameraRect.height = scaledRectHeight;
            cameraRect.y = (1.0f - scaledRectHeight) / 2.0f;
        }
        else if(scaledRectHeight > 1.0f)
        {
            //By inverting the scaledRectHeight we will get the actual difference in width between our old and new aspect ratio.
            var scaledRectWidth = 1.0f / scaledRectHeight;
            
            //We want our camera rect to be stretched fully in height and start at the bottom of the screen
            cameraRect.height = 1.0f;
            cameraRect.y = 0;

            cameraRect.width = scaledRectWidth;
            cameraRect.x = (1.0f - scaledRectWidth) / 2.0f;
        }

        //This will actually change the values in the Camera component.
        gameCamera.rect = cameraRect;

        //Save the last resolution as current 
        _lastResolution = currentResolution;
    }
}
