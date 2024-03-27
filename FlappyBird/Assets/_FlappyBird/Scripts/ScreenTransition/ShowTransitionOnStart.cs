using UnityEngine;

/// <summary>
/// This component will simply start the screen transition with predefined values.
/// </summary>
public class ShowTransitionOnStart : MonoBehaviour
{
    //The reference to the Screen Transition Object
    public ScreenTransitionController screenTransitionController;
    
    //Color at the start of the transition
    public Color fromColor = Color.black;
    
    //Color at the end of the transition
    public Color toColor = Color.clear;
    
    //Should the object of the Screen Transition be disabled at the end of it.
    public bool disableWhenFinished = true;
    
    public void Start()
    {
        //Show transition when object starts.
        screenTransitionController.ShowTransition(fromColor, toColor, disableWhenFinished, null);
    }
}