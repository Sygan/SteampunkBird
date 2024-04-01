using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This script is responsible for providing the main logic for the Bird Game Object
/// It controls jumping, bird death and collecting points
/// </summary>
public class BirdController : MonoBehaviour
{
    //### These variables will show in inspector ###
    
    //When you declare a variable with a public modifier it will be visible in the Inspector window.
    
    
    // This variable defines the force that will be applied to the bird when the PLayer presses space.
    public float jumpForce;

    // This will limit the max velocity of the bird when going up.
    public float maxVelocity;

    // These values define the max/min Z rotation of the birds nose.
    public float birdNoseUpZRotation;
    public float birdNoseDownZRotation;
    
    // This value defines how fast should we rotate our birds nose.
    public float birdNoseRotationChangeSpeed;
    
    // This allows us to set the reference to the Rigidbody2D component which is responsible for handling 2D Physics.
    // We can use it's methods to apply force when the bird jumps.  
    public Rigidbody2D rigidbody;

    // This is a reference to the Animator component that can be used for controlling the animation
    public Animator animator;

    // This is a reference to the AudioSource component that we can use for playing the sound effects for our bird.
    public AudioSource audioSource;

    // These are the references to the audio clips of sound effects 
    public AudioClip jumpSound;
    public AudioClip hitSound;
    public AudioClip pointScoreSound;

    // This is the reference to the UI Game Object that holds is the game over screen.
    public GameObject gameOverScreen;

    // This is the reference to the UI Game Object that holds the how to play screen.
    public GameObject howToPlayScreen;
    
    // This is the variable that keep the current score for the Player.
    public int points;

    //### These variables will be available to use without a need of having a reference to an object, but will not show in Inspector window ###

    // This is a different type of variable than the ones above because it is static. It means that it lives not on the actual object of the Bird in the scene
    // but rather it is accessible without the need of any object existing. 
    public static bool IsDead;

    // This is a so called flag that will check if the game was already started i.e. we've pressed the Jump button for the first time.
    public static bool HasStarted;

    //### These are values private to our object. We can preview their state in Inspector window in Debug View. ###
    
    // We will save the initial gravity scale so we can restore it when the game starts.
    private float initialGravityScale;
    
    // Internally the rotation is handled by using Quaternions, a mathematical structure that allows us to do rotations without experiencing gimbal lock. 
    // This means that we need to either do our rotation maths using Quaternions or convert Euler values into the Quaternions. We will do the latter here.
    private Quaternion birdNoseUpRotation;
    private Quaternion birdNoseDownRotation;

    // This value stores how much should our birds nose be pointed up or down
    // Within values of 0 - 1 where 0 is maximally down and 1 is maximally up.
    private float currentNoseRotationAmount;
    
    /// <summary>
    /// The Start() method is invoked once per object before the first Update()
    /// </summary>
    private void Start()
    {
        //We also need to set the IsDead and HasStarted to false here, because as this variable does not need any actual object to be set, it will keep its value when we restart the level.
        IsDead = false;
        HasStarted = false;

        //We will set the gravityScale to 0 and save it's initial value so the bird doesn't fall at the start of the game. 
        initialGravityScale = rigidbody.gravityScale;
        rigidbody.gravityScale = 0;

        //Here we will convert the euler angles into quaternion values to store our bird rotation.
        //We only need to modify the Z-Axis as we're working in 2D and the x/y are not needed in our case.
        birdNoseUpRotation = Quaternion.Euler(0, 0, birdNoseUpZRotation);
        birdNoseDownRotation = Quaternion.Euler(0, 0, birdNoseDownZRotation);
    }

    /// <summary>
    /// The Update() method is invoked every frame of the game.
    /// </summary>
    private void Update()
    {
        //First, we will check if the Bird is dead. If it is, we will stop executing the gameplay code for the Bird
        //Second, we also want to check if the timeScale == 0 (i.e. game is paused) as timeScale of 0 will still invoke Update method even though physics/deltaTime is paused.
        //Thirdly, we want to check IsPointerOverGameObject() - despite its name this method returns true if our mouse cursor is on top of an UI object.
        //This will keep the bird from jumping in the same moment we're clicking on the Pause Button.
        //Yes, this method name is very badly named
        if(IsDead || Time.timeScale == 0 || EventSystem.current.IsPointerOverGameObject())
            return;
        
        //We can check if the Player invoked the "Jump" button.
        //We're using the old InputManager.API here, the buttons definitions can be found in
        //Edit -> Project Settings -> Input Manager
        if(Input.GetButtonDown("Jump"))
        {
            //If this is our very first Jump, let's start the game
            if (!HasStarted)
            {
                //We will then, set the flag HasStarted to true - as the game has started.
                HasStarted = true;

                //We will also disable the How To Play Screen.
                howToPlayScreen.SetActive(false);
                
                //Restore the gravity scale so the bird can fall down now.
                rigidbody.gravityScale = initialGravityScale;
            }
            
            //We will reset the velocity to 0 so the bird will always jump the same height after falling.
            rigidbody.velocity = Vector2.zero;
            
            //If the Player pressed the button we apply the jumpForce to the Rigidbody2D. 
            //The force can be applied as a 2D Vector of x = 0, and y = jumpForce
            //We also set the ForceMode2D as Impulse, which means that all of the force will be applied instantly to our object.
            rigidbody.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            
            //We will invoke the trigger "FlapWings" in our Animator that will cause the bird to change to WingsUp animation state.
            animator.SetTrigger("FlapWings");
            
            //This code will play the jumpSound using the values set up in our AudioSource component.
            audioSource.PlayOneShot(jumpSound);
        }

        //And we will limit the velocity to a predefined max velocity so the bird will never start going to fast if player presses multiple jumps
        if (rigidbody.velocity.y > maxVelocity)
            rigidbody.velocity = new Vector2(0, maxVelocity);

        //Lastly we will set the birds nose rotation that depends on current bird velocity.
        //But only if we've made the first jump, so it's pointing forward before we start the game.
        if(HasStarted)
            SetTheDirectionTheBirdIsPointing();

    }

    /// <summary>
    /// The method OnCollisionEnter2D is invoked on a object that has the Rigidbody2D component attached to it.
    /// It is invoked when the object hits other any object that has a Collider 2D component attached to it - Eg.: BoxCollider2D
    /// </summary>
    /// <param name="collision">This parameter holds the information about the collision that just happened. We don't need it in this case.</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Don't die again if we are already dead.
        //Otherwise it will play the hit sound, each time we hit new collider.
        if(IsDead)
            return;
        
        //We will mark the player as dead.
        IsDead = true;

        //We will try to set the current points amount as a new highscore 
        TryToSetCurrentHighscore();
        
        //This code will play the hitSound using the values set up in our AudioSource component.
        audioSource.PlayOneShot(hitSound);

        //We will also enable the GameOverScreen game object. It has the SlideIn animation set up as its initial animation so it will
        //automatically slide in after being enabled.
        gameOverScreen.SetActive(true);

    }

    // This method will return the current highscore value.
    public int GetCurrentHighscore()
    {
        //The PlayerPrefs are an easy and quick way of saving some data for our game.
        //They are not as flexible or fast as creating an actual save file, but for small values they are just fine.
        //This value is stored in (on Windows) in system Registry. For the web builds it is stored as a cookie. 
        return PlayerPrefs.GetInt("Highscore");
    }

    // This method will try to set the current highscore value if current score is greater than it.
    private void TryToSetCurrentHighscore()
    {
        //We want to check if the current amount of points is greater than the highscore.
        if (points > GetCurrentHighscore())
        {
            //If it is then we will replace the Highscore value stored on the computer with the current points value.
            PlayerPrefs.SetInt("Highscore", points);
        }
    }
    
    /// <summary>
    /// The method OnTriggerEnter2D is very similar to the OnCollisionEnter2D but it will be invoked if the Collider we've hit
    /// Has the IsTrigger property set up to true. Triggers will send the events that the collision has happened but will not physically
    /// execute the collision i.e. the Rigidbody will not stop after hitting this collision.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //If we hit the point score after death, it will not add the point to the score count
        if(IsDead || !HasStarted)
            return;
        
        //We can check if the trigger that we've just has a Tag of "ScorePoint" defined on it. 
        //We can define the tags in top part of the Inspector window, just under the object name. 
        if(collision.CompareTag("ScorePoint"))
        {
            //If we hit the ScorePoint trigger, we will increase the curren amount of points. 
            points++;
            
            //This code will play the pointScoreSound using the values set up in our AudioSource component.
            audioSource.PlayOneShot(pointScoreSound);
        }
    }

    /// <summary>
    /// This method will point the birds nose up if he's moving up and down when he's moving down. 
    /// </summary>
    private void SetTheDirectionTheBirdIsPointing()
    {
        //We will either increase or decrease the Z rotation amount for our bird's nose 
        if (rigidbody.velocity.y > 0)
        {       
            //If the bird is moving up - it's velocity in Y axis is greater than 0 - increase the value so it will point more up.
            currentNoseRotationAmount += birdNoseRotationChangeSpeed * Time.deltaTime;
        }
        else if (rigidbody.velocity.y < 0)
        {        
            //If the bird is moving down - it's velocity in Y axis is less than 0 - decrease the value so it will point more down.
            currentNoseRotationAmount -= birdNoseRotationChangeSpeed * Time.deltaTime;
        }

        //This method will clamp the value we're receiving in between 0 and 1.
        //Again 0 is for maximum down rotation and 1 is for maximum up rotation.
        currentNoseRotationAmount = Mathf.Clamp01(currentNoseRotationAmount);
        
        //Lerp between nose down rotation and nose up rotation.
        //This will take our currentNoseRotationAmount and set it in between nos down and up rotation depending on it. 
        //i.e. if it was currentNoseRotationAmount was 0.5f it would be exactly in the middle in between birdNoseDownRotation and birdNoseUpRotation. 
        transform.rotation = Quaternion.Lerp(birdNoseDownRotation, birdNoseUpRotation, currentNoseRotationAmount);
    }
}
