using TMPro;
using UnityEngine;

/// <summary>
/// This class is responsible for displaying the current score or highscore.
/// </summary>
public class ScoreController : MonoBehaviour
{
    //Should the object be disable when The Player is dead
    public bool disableWhenPlayerIsDead;
    
    //Should this controller display the highscore value or the current score
    public bool displayHighscore;
    
    //The reference to the BirdController object
    public BirdController birdController;
    
    //The reference to the TextMeshPro component that is used for displaying the score value.
    public TextMeshProUGUI textMeshProUGUI;

    private void Update()
    {
        //If we set this component to disable itself when the player is dead, do it.
        if (disableWhenPlayerIsDead && BirdController.IsDead)
        {
            //This will set the active state of the gameObject (itself) to false. 
            gameObject.SetActive(false);
            return;
        }

        //We use the technique here called polling, i.e. we set the new value to our text component every frame
        //by getting it from some other source. This is not most optimized way of doing this, but for our game it is fine.
        //Normally you would only want to update this text if the score value has actually changed.
        if (displayHighscore)
        {
            textMeshProUGUI.text = birdController.GetCurrentHighscore().ToString();
        }
        else
        {
            textMeshProUGUI.text = birdController.points.ToString();
        }
    }
}
