using UnityEngine;

/// <summary>
/// This component is responsible for disabling object with a slight delay.
/// This is so when we disable the About screen the click sound will play.
/// Otherwise the object will be disabled before the sound finishes.
/// </summary>
public class DisableObjectButtonController : MonoBehaviour
{
    //The reference to the game object we want to disable
    public GameObject gameObjectToDisable;
    
    /// This method is being invoked from the UI Button component in the OnClick() event.
    public void SetActiveToFalse()
    {
        //We can delay execution of a method within the component by using the Invoke method.
        //We can pass the name of this method as a parameter for the function. The second parameters defines the delay after which we want this method 
        //to execute
        Invoke("DisableObject", 0.5f);
    }

    public void DisableObject()
    {
        //Sets the specified Game Object active state to false.
        gameObjectToDisable.SetActive(false);
    }
}
