using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.IO;
using System.Collections;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager instance;

    //da es bereits ein Singelton gibt, benutze ich diesen, um die Information auf dem Server zu uebergeben
    public int chairsNumber;
    public bool chatIsOn;
    public bool voicechatIsOn;
    public string roomCode;

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
        //muss erweitert werden, damit wir mehrere Szenen laden koennten
        if (scene.buildIndex == 1 || scene.buildIndex == 2 )
        {
            StartCoroutine(WaitForConnectionAndInstantiate());
            //PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
        }
    }
    private IEnumerator WaitForConnectionAndInstantiate()
    {
        // Warte, bis der Spieler vollständig mit dem Raum verbunden ist. Sonst wird kein PlayerManager erstellt, da die Verbindung noch nicht vollstandig sei
        while (!PhotonNetwork.InRoom)
        {
            yield return null; // wartet einen Frame und überprüft dann erneut
        }

        // Sobald der Spieler im Raum ist, instanziere den PlayerManager
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
