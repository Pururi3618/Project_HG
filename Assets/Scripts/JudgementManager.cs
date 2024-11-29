using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class JudgementManager : MonoBehaviour
{
    private float perfect = 40f;
    private float great = 60f;
    private float good = 120f;

    public int combo;

    private NoteGenerator noteGenerator;

    public TextMeshProUGUI judgeText;
    public TextMeshProUGUI fastSlow;
    public TextMeshProUGUI comboText;

    private Coroutine currentJudgementRoutine;

    [System.Obsolete]
    private void Start()
    {
        Color tempColor = judgeText.color;
        tempColor.a = 0f;
        judgeText.color = tempColor;
        fastSlow.color = tempColor;
        noteGenerator = FindObjectOfType<NoteGenerator>();
        combo = 0;
    }

    public void Judge(int raneNumber, float currentTimeMs)
    {
        var filteredNotes = noteGenerator.notes
        .Where(note => Mathf.Abs(note.ms - currentTimeMs) <= 2000)
        .ToList();

        foreach (NoteClass note in filteredNotes)
        {
            float timeDifference = Mathf.Abs(note.ms - currentTimeMs);

            if (timeDifference <= perfect && raneNumber + 1 == note.position)
            {
                PerformAction(note, "Perfect", currentTimeMs);
                AddCombo(1);
                break;
            }
            if (timeDifference <= great && raneNumber + 1 == note.position)
            {
                PerformAction(note, "Great", currentTimeMs);
                AddCombo(1);
                break;
            }
            if (timeDifference <= good && raneNumber + 1 == note.position)
            {
                PerformAction(note, "Good", currentTimeMs);
                AddCombo(1);
                break;
            }
        }
    }

    public void AddCombo(int amount)
    {
        combo += amount;
        comboText.text = $"{combo}";
    }

    public void ClearCombo()
    {
        combo = 0;
        comboText.text = $"{combo}";
    }

    public void PerformAction(NoteClass note, string judgement, float currentTimeMs)
    {
        Debug.Log($"{judgement}: {note.ms}, input: {currentTimeMs}");
        float Ms = (int)note.ms - (int)currentTimeMs;
        StartCoroutine(JudegementTextShower(judgement, Ms));
        Destroy(note.noteObject);
    }

    IEnumerator JudegementTextShower(string judgement, float Ms)
    {
        if (currentJudgementRoutine != null)
        {
            StopCoroutine(currentJudgementRoutine);
        }
        currentJudgementRoutine = StartCoroutine(ShowJudgementTextRoutine(judgement, Ms));
        yield break;
    }

    private IEnumerator ShowJudgementTextRoutine(string judgement, float Ms)
    {
        Color tempColor = judgeText.color;
        tempColor.a = 1f;
        judgeText.color = tempColor;
        judgeText.text = $"{judgement}";
        if (Ms > 0)
        {
            fastSlow.color = tempColor;
            fastSlow.text = $"+{(int)Ms}";
        }
        if (Ms < 0)
        {
            fastSlow.color = tempColor;
            fastSlow.text = $"{(int)Ms}";
        }

        yield return new WaitForSeconds(2f);

        tempColor = judgeText.color;
        tempColor.a = 0;
        judgeText.color = tempColor;
        fastSlow.color = tempColor;
        currentJudgementRoutine = null;
    }

}