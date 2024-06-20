using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.IO;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager instance;

    //da es bereits ein Singelton gibt, benutze ich diesen, um die Information auf dem Server zu uebergeben
    public int chairsNumber;
    public bool chatIsOn;
    public bool voicechatIsOn;

    private void Start()
    {
        //schaut, ob ein anderen RoomManger existiert
        if (instance)
        {
            Destroy(gameObject);
            return;
        }
        //damit wenn wir Scene wechseln, RoomManager das gleiche bleibt
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
        if (scene.buildIndex == 1)
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
        }
    }  

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
