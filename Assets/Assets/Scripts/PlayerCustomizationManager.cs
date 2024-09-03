using UnityEngine;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerCustomizationManager : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    //um alle Objekte in Prefab zu speichern
    public GameObject[] hats;
    public GameObject[] eyes;
    public GameObject[] bodies;
    public GameObject[] clothes;

    private int[] lastCustomization = { 0, 0, 0, 0, 100, 37, 30}; //hat, eyes, body, clothes, redHat, greenHat, blueHat

    public CharacterEditingMenu editingMenu;

    void Start()
    {
        SetDefaultProperties();
        ApplyCustomization();
        /*if (photonView.IsMine)
        {
            SetDefaultProperties();
            ApplyCustomization();
        }*/
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        int hatIndex = (int)info.photonView.InstantiationData[1];
        int eyeIndex = (int)info.photonView.InstantiationData[2];
        int bodyIndex = (int)info.photonView.InstantiationData[3];
        int clothesIndex = (int)info.photonView.InstantiationData[4];

        EquipItem(hats, hatIndex);
        EquipItem(eyes, eyeIndex);
        EquipItem(bodies, bodyIndex);
        EquipItem(clothes, clothesIndex);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (targetPlayer == photonView.Owner)
        {
            if (changedProps.ContainsKey("HatIndex") || changedProps.ContainsKey("EyeIndex") ||
                changedProps.ContainsKey("BodyIndex") || changedProps.ContainsKey("ClothesIndex"))
            {
                ApplyCustomization();
                //??? Braucht man das hier
                editingMenu.ApplyCustomHatFromHash(changedProps);
            }
        }
    }

    public void ApplyCustomization()
    {
        Hashtable playerProperties = PhotonNetwork.LocalPlayer.CustomProperties;
        //falls eine Liste leer ist
        if (playerProperties.ContainsKey("HatIndex") && hats.Length > 0)
        {
            EquipItem(hats, (int)playerProperties["HatIndex"]);
        }
        UpdateHatColor((int)playerProperties["HatColorR"], (int)playerProperties["HatColorG"], (int)playerProperties["HatColorB"]);

        if (playerProperties.ContainsKey("EyeIndex") && eyes.Length > 0)
        {
            EquipItem(eyes, (int)playerProperties["EyeIndex"]);
        }

        if (playerProperties.ContainsKey("BodyIndex") && bodies.Length > 0)
        {
            EquipItem(bodies, (int)playerProperties["BodyIndex"]);
        }

        if (playerProperties.ContainsKey("ClothesIndex") && clothes.Length > 0)
        {
            EquipItem(clothes, (int)playerProperties["ClothesIndex"]);
        }
    }

    private void UpdateHatColor(int red, int green, int blue)
    {
        GameObject equipedHat = hats[0];
        for (int i = 0; i < hats.Length; i++)
        {
            if (hats[i].activeSelf)
            {
                equipedHat = hats[i];
            }
        }
        Renderer equipedHatRenderer = equipedHat.GetComponent<Renderer>();
        if (equipedHatRenderer == null || !equipedHatRenderer.material.HasProperty("_Color"))
        {
            Debug.Log("Farbe kann nicht gesetzt werden, weil Renderer oder Material fehlt");
            return;
        }

        Color newColor = new Color((red / 255f), (green / 255f), (blue / 255f));

        //erstellt eine neue Material-Instanz, um das Originalmaterial nicht zu aendern
        Material newMaterial = new Material(equipedHatRenderer.material);
        newMaterial.color = newColor;

        //setzt das neue Material auf den Renderer
        equipedHatRenderer.material = newMaterial;
    }

    private void EquipItem(GameObject[] items, int index)
    {
        //falls in der Liste nichts enthalten ist
        if (items.Length == 0) return;

        foreach (var item in items)
        {
            item.SetActive(false);
        }

        if (index >= 0 && index < items.Length)
        {
            items[index].SetActive(true);
        }
    }

    private void SetDefaultProperties()
    {
        Hashtable playerProperties = PhotonNetwork.LocalPlayer.CustomProperties;
        bool propertiesUpdated = false;

        if (!playerProperties.ContainsKey("HatIndex"))
        {
            playerProperties["HatIndex"] = 0;  //default value
            propertiesUpdated = true;
        }

        if (!playerProperties.ContainsKey("EyeIndex"))
        {
            playerProperties["EyeIndex"] = 0;  //default value
            propertiesUpdated = true;
        }

        if (!playerProperties.ContainsKey("BodyIndex"))
        {
            playerProperties["BodyIndex"] = 0;  //default value
            propertiesUpdated = true;
        }

        if (!playerProperties.ContainsKey("ClothesIndex"))
        {
            playerProperties["ClothesIndex"] = 0;  //default value
            propertiesUpdated = true;
        }

        if (!playerProperties.ContainsKey("HatColorR"))
        {
            playerProperties["HatColorR"] = 100;
            propertiesUpdated = true;
        }

        if (!playerProperties.ContainsKey("HatColorG"))
        {
            playerProperties["HatColorG"] = 37;
            propertiesUpdated = true;
        }

        if (!playerProperties.ContainsKey("HatColorB"))
        {
            playerProperties["HatColorB"] = 30;
            propertiesUpdated = true;
        }

        if (propertiesUpdated)
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
        }
    }

    public void SaveLastCustomization()
    {
        Hashtable playerProperties = PhotonNetwork.LocalPlayer.CustomProperties;
        lastCustomization[0] = (playerProperties.ContainsKey("HatIndex") && hats.Length > 0) ? (int)playerProperties["HatIndex"] : 0;
        lastCustomization[1] = (playerProperties.ContainsKey("EyeIndex") && eyes.Length > 0) ? (int)playerProperties["EyeIndex"] : 0;
        lastCustomization[2] = (playerProperties.ContainsKey("BodyIndex") && bodies.Length > 0) ? (int)playerProperties["BodyIndex"] : 0;
        lastCustomization[3] = (playerProperties.ContainsKey("ClothesIndex") && clothes.Length > 0) ? (int)playerProperties["ClothesIndex"] : 0;
        lastCustomization[4] = playerProperties.ContainsKey("HatColorR") ? (int)playerProperties["HatColorR"] : 100;
        lastCustomization[5] = playerProperties.ContainsKey("HatColorG") ? (int)playerProperties["HatColorG"] : 37;
        lastCustomization[6] = playerProperties.ContainsKey("HatColorB") ? (int)playerProperties["HatColorB"] : 30;
    }
    public void ResetCustomizationToLast()
    {
        Hashtable playerProperties = PhotonNetwork.LocalPlayer.CustomProperties;
        playerProperties["HatIndex"] = lastCustomization[0];
        playerProperties["EyeIndex"] = lastCustomization[1];
        playerProperties["BodyIndex"] = lastCustomization[2];
        playerProperties["ClothesIndex"] = lastCustomization[3];
        playerProperties["HatColorR"] = lastCustomization[4];
        playerProperties["HatColorG"] = lastCustomization[5];
        playerProperties["HatColorB"] = lastCustomization[6];
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);

        ApplyCustomization();
        editingMenu.ApplyCustomHatFromHash(playerProperties);
    }
}