using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [Header("Joueur 1")]
    public GameObject voiture1;
    public Camera camera1;

    [Header("Joueur 2")]
    public GameObject voiture2;
    public Camera camera2;

    void Start()
    {
        UpdateLayout();
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        // Touches de test : F1 / F2 pour activer/désactiver
        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            voiture1.SetActive(!voiture1.activeSelf);
            UpdateLayout();
        }
        if (Keyboard.current.f2Key.wasPressedThisFrame)
        {
            voiture2.SetActive(!voiture2.activeSelf);
            UpdateLayout();
        }
    }

    void UpdateLayout()
    {
        bool j1 = voiture1.activeSelf;
        bool j2 = voiture2.activeSelf;

        if (j1 && j2)
        {
            camera1.rect = new Rect(0f, 0f, 0.5f, 1f);
            camera2.rect = new Rect(0.5f, 0f, 0.5f, 1f);
        }
        else if (j1)
        {
            camera1.rect = new Rect(0f, 0f, 1f, 1f);
        }
        else if (j2)
        {
            camera2.rect = new Rect(0f, 0f, 1f, 1f);
        }
    }
}