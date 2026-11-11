using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueTrigger : MonoBehaviour
{
    // Se conserva al recargar la escena (hasta que cierras el juego)
    private static readonly HashSet<string> seenDialogues = new HashSet<string>();

    public enum Mode { Touch, Interact }

    [SerializeField] private Mode mode = Mode.Touch;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private bool onlyOnce = true;       // se ve una sola vez, aunque reinicies el nivel
    [SerializeField] private string dialogueId = "";     // opcional: nombre único del diálogo
    [SerializeField] private GameObject prompt;          // opcional: icono "E"
    [SerializeField, TextArea(2, 5)] private string[] lines;

    private bool playerInside;
    private bool used;

    private string Id =>
        string.IsNullOrEmpty(dialogueId)
            ? SceneManager.GetActiveScene().name + "/" + gameObject.name
            : dialogueId;

    void Awake()
    {
        // Si ya se vio en un intento anterior, este trigger queda inactivo
        if (onlyOnce && seenDialogues.Contains(Id))
            used = true;
    }

    void Update()
    {
        if (mode == Mode.Interact && playerInside && !used && Input.GetKeyDown(interactKey))
            Talk();
    }

    private void Talk()
    {
        if (DialogueManager.Instance == null)
        {
            Debug.LogWarning("No hay ningún DialogueManager activo en la escena");
            return;
        }

        DialogueManager.Instance.StartDialogue(lines);

        if (onlyOnce)
        {
            used = true;
            seenDialogues.Add(Id);
            ShowPrompt(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = true;
        if (used) return;

        if (mode == Mode.Touch) Talk();
        else ShowPrompt(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;
        ShowPrompt(false);
    }

    private void ShowPrompt(bool show)
    {
        if (prompt != null) prompt.SetActive(show);
    }

    // Llamar para volver a mostrar todos los diálogos (por ejemplo, en "Nueva partida")
    public static void ResetSeen()
    {
        seenDialogues.Clear();
    }
}