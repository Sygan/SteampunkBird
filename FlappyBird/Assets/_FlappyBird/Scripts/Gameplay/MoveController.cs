using UnityEngine;

/// <summary>
/// This component is responsible for moving the objects in our world and looping their movement thus making the illusion
/// that the player is flying right, when in fact it is the world that moves towards the player and loops around him indefinitely.
/// We use it for all of our objects that are moving left such as Pipes, Backgrounds, Ground etc.
/// Additionally it allows us to randomize the Y-Axis position of the Pipes so the gameplay is more fun!  
/// </summary>
public class MoveController : MonoBehaviour
{
    // This variable defines the speed the objects use to move in our game
    public float speed;   
    
    // At which position in the X-Axis we want our object to re-appear
    public float spawnPositionX;
    
    // At which position in the X-Axis we want our object to disappear
    public float despawnPositionX;

    // This variable allows us to define do we want to randomize the Y-Axis spawn position. 
    public bool randomizeTheYPosition;
    
    // These two values define the range between which we can randomize the object position in the Y-Axis   
    public float yMin;
    public float yMax;

    // If this is set to true, then instead of teleporting the object to spawn position, we will destroy it.
    public bool destroyWhenReachedDespawnPoint;

    // If this is set to true the controller will always move the object, no matter the state of the Player. 
    public bool alwaysMove;
    
    private void Start()
    {
        //If we want to randomize the Y position, do it
        if (randomizeTheYPosition)
        {
            //This is used by our pipes to randomize their Y-Axis position on the start of the game.
            transform.position = new Vector3(transform.position.x, Random.Range(yMin, yMax), transform.position.z);
        }
    }

    private void Update()
    {
        //We want the object to stop moving if the player died. 
        //By placing return after the if, we will exit the Update method and not execute the rest of the code.
        //We also don't want the objects to move before we've made our first jump.
        if(!alwaysMove && (BirdController.IsDead || !BirdController.HasStarted))
            return;
        
        //We will move our object in the X-axis by the value of speed.
        //Because Update() method is invoked every frame we want to multiply our speed by the Time.deltaTime value.
        //Time.deltaTime returns how much time has elapsed between previous and current frame.
        //By multiplying our speed * Time.deltaTime we will make our object move with the value of speed per second instead of speed per frame.
        transform.position = new Vector3(transform.position.x + speed * Time.deltaTime, 
            transform.position.y, transform.position.z);

        //If our object reached the X-Axis value of the despawnPositionX we want to  
        if(transform.position.x < despawnPositionX)
        {
            if (destroyWhenReachedDespawnPoint)
            {
                //Either destroy it if this variable is set to true.
                Destroy(gameObject);
            }
            else
            {
                if (randomizeTheYPosition)
                {
                    //Or teleport it back to the X-Axis value of spawnPositionX and randomize the Y-Axis value to be between yMin and yMax.
                    transform.position = new Vector3(spawnPositionX, Random.Range(yMin, yMax), transform.position.z);
                }
                else
                {
                    //If we don't want to randomize the value, then just teleport it keeping the same Y-axis position.
                    transform.position = new Vector3(spawnPositionX, transform.position.y, transform.position.z);
                }
            }
        }
    }
}
