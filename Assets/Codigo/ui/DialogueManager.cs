using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private float typingSpeed = 0.03f; // segundos por letra
    [SerializeField] private KeyCode nextKey = KeyCode.E;
    [SerializeField] private bool pauseGame = true;      // congela el juego mientras hablas

    private string[] lines;
    private int index;
    private bool isActive;
    private bool typing;
    private Coroutine typingRoutine;
    private int startFrame = -1;
    private int endFrame = -1;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void StartDialogue(string[] newLines)
    {
        // Evita que la misma pulsación de E que abre o cierra el diálogo lo vuelva a abrir
        if (isActive || Time.frameCount == endFrame) return;
        if (newLines == null || newLines.Length == 0) return;

        lines = newLines;
        index = 0;
        isActive = true;
        startFrame = Time.frameCount;

        panel.SetActive(true);
        if (pauseGame) Time.timeScale = 0f;

        ShowLine();
    }

    void Update()
    {
        if (!isActive || Time.frameCount == startFrame) return;
        if (!Input.GetKeyDown(nextKey)) return;

        if (typing)
        {
            // Primer E: completa la línea al instante
            StopCoroutine(typingRoutine);
            dialogueText.maxVisibleCharacters = int.MaxValue;
            typing = false;
        }
        else
        {
            // Segundo E: pasa a la siguiente línea o cierra
            index++;
            if (index < lines.Length) ShowLine();
            else EndDialogue();
        }
    }

    private void ShowLine()
    {
        typingRoutine = StartCoroutine(TypeLine(lines[index]));
    }

    private IEnumerator TypeLine(string line)
    {
        typing = true;
        dialogueText.text = line;
        dialogueText.maxVisibleCharacters = 0;
        dialogueText.ForceMeshUpdate();

        int total = dialogueText.textInfo.characterCount;
        for (int i = 1; i <= total; i++)
        {
            dialogueText.maxVisibleCharacters = i;
            yield return new WaitForSecondsRealtime(typingSpeed); // funciona con el juego pausado
        }

        typing = false;
    }

    private void EndDialogue()
    {
        isActive = false;
        endFrame = Time.frameCount;
        panel.SetActive(false);
        if (pauseGame) Time.timeScale = 1f;
    }
}