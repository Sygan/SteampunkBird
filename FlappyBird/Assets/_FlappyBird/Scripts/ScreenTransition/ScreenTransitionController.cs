using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// This component is responsible for showing the transition screen and sending an event when the transition has finished.
/// </summary>
public class ScreenTransitionController : MonoBehaviour
{
    //The speed of our transition
    public float transitionSpeed;
    
    //The target image component for our transition to use
    public Image obscurerImage;
    
    //We can invoke this method to show the transition
    public void ShowTransition(Color fromColor, Color toColor, bool disableWhenFinished, UnityAction onFinished)
    {
        //We will enable the transition game object 
        gameObject.SetActive(true);
        
        //We will start the coroutine that is doing the actual transition.
        StartCoroutine(DoTransition(fromColor, toColor, disableWhenFinished, onFinished));
    }
    
    //Coroutines are one of the ways in Unity how we can make asynchronous code. 
    //They are not actually asynchronous but rather they use a clever implementation to simulate running
    //async code but they are enough in a lot of cases where we don't want to deal with actual async code.
    //You can read more about them in the Unity docs if you're interested.
    private IEnumerator DoTransition(Color fromColor, Color toColor, bool disableWhenFinished, UnityAction onFinished)
    {
        //We will set up some initial values for our transition such as starting color for our Image.
        obscurerImage.color = fromColor;
        var currentLerp = 0f;
        
        //We can set up "infinite" loop here as we will break out of it manually.
        while (true)
        {
            //We will increase our lerp value
            currentLerp += transitionSpeed * Time.deltaTime;
            
            //Now in the loop we will lerp between two colors with our predefined speed.
            obscurerImage.color = Color.LerpUnclamped(obscurerImage.color, toColor, currentLerp);

            //If our current lerp value reaches one this means the transition is over.
            if (currentLerp >= 1.0f)
                break;

            //And this is the actual clever thing about Coroutines. In theory we are in a while loop so we shouldn't be able to execute any 
            //code until this loop finishes. Coroutines allows us to actually pause their execution until some condition is fulfilled. 
            //In our case we will pause the execution of this loop and return to it after one frame has passed.
            //It will simulate the behavior of the Update() method.
            yield return new WaitForEndOfFrame();
        }
        
        //We need to check if any event was actually set up after we exit our loop.
        if (onFinished != null)
        {
            //If it was, we will invoke this event.
            onFinished.Invoke();
        }

        //We will disable the transition game object at the end if specified
        if (disableWhenFinished)
        {
            gameObject.SetActive(false);
        }
    }
    
}
