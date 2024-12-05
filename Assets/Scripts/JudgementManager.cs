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
    private float good = 110f;
    private float bad = 140f;

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
        ClearCombo();
    }

    public void Judge(int raneNumber, float currentTimeMs)
    {
        var filteredNotes = noteGenerator.notes
        .Where(note => Mathf.Abs(note.ms - currentTimeMs) <= 1000)
        .ToList();

        foreach (NoteClass note in filteredNotes)
        {
            float timeDifference = Mathf.Abs(note.ms - currentTimeMs);

            if (timeDifference <= bad && note.type == "hold" && raneNumber + 1 == note.position && !note.isInputed)
            {
                PerformAction(note, "Perfect", note.ms);
                AddCombo(1);
                break;
            }
            if (timeDifference <= perfect && note.type == "normal" && raneNumber + 1 == note.position && !note.isInputed)
            {
                PerformAction(note, "Perfect", currentTimeMs);
                AddCombo(1);
                break;
            }
            if (timeDifference <= great && note.type == "normal" && raneNumber + 1 == note.position && !note.isInputed)
            {
                PerformAction(note, "Great", currentTimeMs);
                AddCombo(1);
                break;
            }
            if (timeDifference <= good && note.type == "normal" && raneNumber + 1 == note.position && !note.isInputed)
            {
                PerformAction(note, "Good", currentTimeMs);
                AddCombo(1);
                break;
            }
            if (timeDifference <= bad && note.type == "normal" && raneNumber + 1 == note.position && !note.isInputed)
            {
                PerformAction(note, "Bad", currentTimeMs);
                ClearCombo();
                break;
            }
        }
    }

    public void UpJudge(int raneNumber, float currentTimeMs)
    {
        var filteredNotes = noteGenerator.notes
        .Where(note => Mathf.Abs(note.ms - currentTimeMs) <= 1000)
        .ToList();

        foreach (NoteClass note in filteredNotes)
        {
            float timeDifference = Mathf.Abs(note.ms - currentTimeMs);
            float notAbsDiff = note.ms - currentTimeMs;

            if (timeDifference <= perfect && note.type == "up" && raneNumber + 1 == note.position && !note.isInputed)
            {
                PerformAction(note, "Perfect", currentTimeMs);
                AddCombo(1);
                break;
            }
            if (notAbsDiff >= -80 && notAbsDiff < -40 && note.type == "up" && raneNumber + 1 == note.position && !note.isInputed)
            {
                PerformAction(note, "Perfect", currentTimeMs);
                AddCombo(1);
                break;
            }
            if (notAbsDiff >= -120 && notAbsDiff < -80 && note.type == "up" && raneNumber + 1 == note.position && !note.isInputed)
            {
                PerformAction(note, "Great", currentTimeMs);
                AddCombo(1);
                break;
            }
            if (notAbsDiff >= -160 && notAbsDiff < -120 && note.type == "up" && raneNumber + 1 == note.position && !note.isInputed)
            {
                PerformAction(note, "Good", currentTimeMs);
                AddCombo(1);
                break;
            }
            if (notAbsDiff >= -200 && notAbsDiff < -160 && note.type == "up" && raneNumber + 1 == note.position && !note.isInputed)
            {
                PerformAction(note, "Bad", currentTimeMs);
                ClearCombo();
                break;
            }
            if (notAbsDiff > 40 && notAbsDiff <= 50  && note.type == "up" && raneNumber + 1 == note.position && !note.isInputed)
            {
                PerformAction(note, "Great", currentTimeMs);
                AddCombo(1);
                break;
            }
            if (notAbsDiff > 50 && notAbsDiff <= 60 && note.type == "up" && raneNumber + 1 == note.position && !note.isInputed)
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
        comboText.text = $"";
    }

    public void PerformAction(NoteClass note, string judgement, float currentTimeMs)
    {
        Debug.Log($"{judgement}: {note.ms}, input: {currentTimeMs}");
        note.isInputed = true;
        Destroy(note.noteObject);
        float Ms = note.ms - currentTimeMs;
        StartCoroutine(JudegementTextShower(judgement, Ms));
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
        if (Ms == 0)
        {
            fastSlow.color = tempColor;
            fastSlow.text = $"";
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
