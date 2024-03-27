using UnityEngine;

/// <summary>
/// This component is responsible from pausing/unpausing the game by pressing the UI button.
/// </summary>
public class PauseButtonController : MonoBehaviour
{
    /// <summary>
    /// This variable holds the information if the game is currently paused. If not defined, it has default value of false when we start the game.
    /// </summary>
    private bool _isPaused;
    
    /// <summary>
    /// This method is being invoked from the UI Button component in the OnClick() event.
    /// You can find this component on the PauseButton Game Object
    /// </summary>
    public void SwitchPauseState()
    {
        //If the Player has died or the gameplay has not yet been started, do nothing 
        if(BirdController.IsDead || !BirdController.HasStarted)
            return;
        
        //We will invert current state of _isPaused variable
        _isPaused = !_isPaused;

        //Depending on the current state of the _isPaused variable we will either change the timeScale to 0 or 1, thus 
        //pausing or unpausing current game
        Time.timeScale = _isPaused ? 0f : 1f;
    }
}
