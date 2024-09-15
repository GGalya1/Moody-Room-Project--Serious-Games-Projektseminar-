using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

/// <summary>
/// Manages player customization, including the selection and application of hats, eyes, bodies, clothes, and hat colors.
/// This script handles both local player customization and synchronizing these choices.
/// </summary>
public class PlayerCustomizationManager : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    // Arrays to hold references to customization items like hats, eyes, bodies, and clothes.
    public GameObject[] hats;
    public GameObject[] eyes;
    public GameObject[] bodies;
    public GameObject[] clothes;

    // Array to store last customization state: [hat, eyes, body, clothes, redHat, greenHat, blueHat]
    private int[] lastCustomization = { 0, 0, 0, 0, 100, 37, 30}; 

    public CharacterEditingMenu editingMenu;

    /*void Start()
    {
        SetDefaultProperties();
        ApplyCustomization();
        //if (photonView.IsMine)
        //{
        //    SetDefaultProperties();
        //    ApplyCustomization();
        //}
    }*/

    /// <summary>
    /// Called when the local player object is instantiated.
    /// Applies the customization settings stored in the instantiation data to the player object.
    /// </summary>
    /// <param name="info">Photon message info containing instantiation data.</param>
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        int hatIndex = (int)info.photonView.InstantiationData[1];
        int eyeIndex = (int)info.photonView.InstantiationData[2];
        int bodyIndex = (int)info.photonView.InstantiationData[3];
        int clothesIndex = (int)info.photonView.InstantiationData[4];

        // Set the player's customization settings based on the instantiation data
        EquipItem(hats, hatIndex);
        EquipItem(eyes, eyeIndex);
        EquipItem(bodies, bodyIndex);
        EquipItem(clothes, clothesIndex);
    }

    /// <summary>
    /// Called whenever a player's custom properties are updated.
    /// This method checks if the customization properties have changed and applies the new customization if necessary.
    /// </summary>
    /// <param name="targetPlayer">The player whose properties were updated.</param>
    /// <param name="changedProps">The properties that were changed.</param>
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (targetPlayer == photonView.Owner) // only update 
        {
            if (changedProps.ContainsKey("HatIndex") || changedProps.ContainsKey("EyeIndex") ||
                changedProps.ContainsKey("BodyIndex") || changedProps.ContainsKey("ClothesIndex"))
            {
                ApplyCustomization();
                // Update the editing menu with the new customization values
                editingMenu.ApplyCustomHatFromHash(changedProps);
            }
        }
    }

    /// <summary>
    /// Applies the current customization settings stored in the local player's properties to their character.
    /// </summary>
    public void ApplyCustomization()
    {
        Hashtable playerProperties = PhotonNetwork.LocalPlayer.CustomProperties;
        // Equip the items if the properties exist and there are items to choose from
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

    /// <summary>
    /// Updates the color of the player's hat based on the provided RGB values.
    /// </summary>
    /// <param name="red">Red component of the color (0-255).</param>
    /// <param name="green">Green component of the color (0-255).</param>
    /// <param name="blue">Blue component of the color (0-255).</param>
    public void UpdateHatColor(int red, int green, int blue)
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

        // Create a new material instance to not change the original material
        Material newMaterial = new Material(equipedHatRenderer.material);
        newMaterial.color = newColor;

        // Apply the new material to the renderer
        equipedHatRenderer.material = newMaterial;
    }

    /// <summary>
    /// Activates the item at the specified index in the array of items, deactivating all other items.
    /// </summary>
    /// <param name="items"> Array of items to choose from. </param>
    /// <param name="index"> Index of the item to activate. </param>
    public void EquipItem(GameObject[] items, int index)
    {
        if (items.Length == 0) return; // no items to equip

        foreach (var item in items)
        {
            item.SetActive(false);
        }

        if (index >= 0 && index < items.Length)
        {
            Debug.Log("Ich habe gesetzt: " + items[index].name);
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

    /// <summary>
    /// Saves the current customization settings to the local <see cref="lastCustomization"/> array.
    /// </summary>
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

    /// <summary>
    /// Resets the player's customization to the last saved state.
    /// </summary>
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