using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.IO;
using System.Collections;

/// <summary>
/// The RoomManager class handles scene management and ensures that key multiplayer components like the PlayerManager 
/// are instantiated properly after the player connects to a room in the Photon network. This class also follows the singleton pattern to 
/// make sure one instance exists throughout all the scenes.
/// </summary>
public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager instance;

    //the singleton pattern is used to pass the information to the server
    public int chairsNumber;
    public bool chatIsOn;
    public bool voicechatIsOn;
    public string roomCode;

    private void Start()
    {
        //check if the instance already exists
        if (instance)
        {
            Destroy(gameObject);
            return;
        }
        //if not, create the instance and make it persistent between scenes
        DontDestroyOnLoad(gameObject);
        instance = this;
    }
    
 
    public override void OnEnable() 
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        //can be extented to be able to handle more scenes
        if (scene.buildIndex == 1 || scene.buildIndex == 2)
        {
            StartCoroutine(WaitForConnectionAndInstantiate());
            //PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
        }
    }
    private IEnumerator WaitForConnectionAndInstantiate()
    {
        // waits until the player is fully connected to the room,
        // otherwise the PlayerManager is not created,
        // because the connection is not fully established yet
        while (!PhotonNetwork.InRoom)
        {
            yield return null; // wait for the next frame to check again
        }

        // Once the player is in the room, instantiate the PlayerManager
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
    }

    
    public override void OnDisable()//unregister the event
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
