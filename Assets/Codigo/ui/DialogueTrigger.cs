using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public enum Mode { Touch, Interact }

    [SerializeField] private Mode mode = Mode.Touch;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private bool onlyOnce = true;
    [SerializeField] private GameObject prompt; // opcional: icono "E" sobre el objeto
    [SerializeField, TextArea(2, 5)] private string[] lines;

    private bool playerInside;
    private bool used;

    void Update()
    {
        if (mode == Mode.Interact && playerInside && !used && Input.GetKeyDown(interactKey))
            Talk();
    }

    private void Talk()
    {
        if (DialogueManager.Instance == null) return;

        DialogueManager.Instance.StartDialogue(lines);

        if (onlyOnce)
        {
            used = true;
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
}