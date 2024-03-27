using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This component is responsible for loading the scene when the button is pressed.
/// </summary>
public class LoadButtonController : MonoBehaviour
{
    //The name of the scene that we want to load
    public string sceneName;

    //The reference to the Screen Transition object
    public ScreenTransitionController screenTransition;
    
    //The variable that is set when this button was already pressed.
    //Because this is a private variable of a component after we reload the scene it will be set to false.
    private bool wasPressed;
    
    /// This method is being invoked from the UI Button component in the OnClick() event.
    public void LoadScene()
    {
        //This will prevent the Player for quickly clicking the button and starting the transition multiple times.
        if(wasPressed)
            return;

        //Set the button as already pressed
        wasPressed = true;
        
        //We will start the screen transition from transparent color to black
        //and pass the OnTransitionFinished method to be invoked after the transition has been finished.
        //We don't want to disable the screen transition at the end as we don't want the screen to show other objects just before loading the game.
        screenTransition.ShowTransition(Color.clear, Color.black, false,  OnTransitionFinished);
    }

    private void OnTransitionFinished()
    {
        //We can use this method to load a scene by name.
        //In order for this to work, the scene needs to be added to Build Setting under File->Build Settings menu.
        SceneManager.LoadScene(sceneName);
    }
}
