using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;

public class SongListShower : MonoBehaviour
{
    private MenuManager menu;

    private LoadAllJSONs loader;

    public GameObject contentFolder;
    public GameObject SongListFolder;
    public GameObject songPrefab;
    public GameObject canvas;

    public GameObject syncInput;
    public GameObject speedInput;

    private float originX;

    public int listNum;

    private Coroutine currentSetSongRoutine;
    //private Coroutine currentSetSongIndexRoutine;

    [System.Obsolete]
    private void Start()
    {
        canvas.transform.localScale = Vector3.one;

        loader = FindObjectOfType<LoadAllJSONs>();
        menu = FindObjectOfType<MenuManager>();

        contentFolder.transform.position = new Vector3(contentFolder.transform.position.x, -1f * (SongListFolder.transform.position.y / 2f) + (songPrefab.GetComponent<RectTransform>().sizeDelta.y / 2f), 0f);

        originX = contentFolder.transform.position.x;

        listNum = 0;
    }

    public void Shower()
    {
        foreach (SongInfoClass info in loader.songList)
        {
            Debug.Log($"{info.artist} {info.title} {info.bpm}");

            GameObject song = Instantiate(songPrefab, contentFolder.transform);
            Transform artistTitle = song.transform.Find("Artist-TitlePanel");
            artistTitle.Find("TitleText").gameObject.GetComponent<TextMeshProUGUI>().text = info.title;
            artistTitle.Find("ArtistText").gameObject.GetComponent<TextMeshProUGUI>().text = info.artist;

            song.transform.Find("BPMText").gameObject.GetComponent<TextMeshProUGUI>().text = $"{info.bpm}BPM";

            song.GetComponent<SongListInfoSetter>().artist = info.artist;
            song.GetComponent<SongListInfoSetter>().title = info.title;
        }
    }

    public MainInputAction action;
    private InputAction listUp;
    private InputAction listDown;
    private InputAction songSelect;
    private InputAction exitSongList;

    private void Awake()
    {
        action = new MainInputAction();
        listUp = action.FreePlay.ListUp;
        listDown = action.FreePlay.ListDown;
        songSelect = action.FreePlay.SongSelect;
        exitSongList = action.FreePlay.ExitSongList;
    }

    private void OnEnable()
    {
        listUp.Enable();
        listUp.started += Started;

        listDown.Enable();
        listDown.started += Started;

        songSelect.Enable();
        songSelect.started += Started;

        exitSongList.Enable();
        exitSongList.started += Started;
    }

    private void OnDisable()
    {
        listUp.Disable();
        listUp.started -= Started;

        listDown.Disable();
        listDown.started -= Started;

        songSelect.Disable();
        songSelect.started -= Started;

        exitSongList.Disable();
        exitSongList.started -= Started;
    }

    void Started(InputAction.CallbackContext context)
    {
        string actionName = context.action.name;

        switch (actionName)
        {
            case "ListUp":
                SetList(listNum - 1);
                break;
            case "ListDown":
                SetList(listNum + 1);
                break;
            case "SongSelect":
                SelectSong(listNum);
                break;
            case "ExitSongList":
                Destroy(menu.gameObject);
                SceneManager.LoadScene("Menu");
                break;
        }
    }

    private void SetList(int n)
    {
        int targetIndex = n - listNum;

        Debug.Log(targetIndex);

        //contentFolder.transform.position = new Vector3(contentFolder.transform.position.x, contentFolder.transform.position.y + (songPrefab.GetComponent<RectTransform>().sizeDelta.y * targetIndex), 0f);
        if (currentSetSongRoutine == null)
        {
            listNum = n;

            Debug.Log(n);
            currentSetSongRoutine = StartCoroutine(SetSong(targetIndex));
        }

        //if (currentSetSongIndexRoutine == null)
        //{
        //    currentSetSongIndexRoutine = StartCoroutine(SetSongIndex(contentFolder.transform.GetChild(listNum).gameObject));
        //}
    }

    //private IEnumerator SetSongIndex(GameObject song)
    //{
    //    canvas.transform.localScale = Vector3.one;

    //    Transform T = song.transform;

    //    float elapsedTime = 0f;
    //    Vector3 startPos = new Vector3(originX, T.position.y, 0f);
    //    float duration = 0.25f;
    //    Vector3 targetPos = new Vector3(originX - 20f, T.position.y, 0f);

    //    while (elapsedTime < duration)
    //    {
    //        canvas.transform.localScale = Vector3.one;

    //        elapsedTime += Time.deltaTime;
    //        float t = Mathf.Clamp01(elapsedTime / duration);

    //        float easedT = t;
    //        easedT = Mathf.Sin(t * Mathf.PI * 0.5f);

    //        T.position = Vector3.Lerp(startPos, targetPos, easedT);

    //        yield return null;
    //    }

    //    canvas.transform.localScale = Vector3.one;
    //    T.position = targetPos;

    //    currentSetSongIndexRoutine = null;

    //    yield break;
    //}

    private IEnumerator SetSong(int index)
    {
        canvas.transform.localScale = Vector3.one;

        Transform T = contentFolder.transform;

        float elapsedTime = 0f;
        Vector3 startPos = new Vector3(T.position.x, T.position.y, 0f);
        float duration = 0.25f;
        Vector3 targetPos = new Vector3(originX, T.position.y + (songPrefab.GetComponent<RectTransform>().sizeDelta.y * index), 0f);

        while (elapsedTime < duration)
        {
            canvas.transform.localScale = Vector3.one;

            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            float easedT = Mathf.Sin(t * Mathf.PI * 0.5f);

            T.position = Vector3.Lerp(startPos, targetPos, easedT);

            yield return null;
        }

        canvas.transform.localScale = Vector3.one;
        T.position = targetPos;

        currentSetSongRoutine = null;

        yield break;
    }

    public void SelectSong(int n)
    {
        Debug.Log(n);
        SongListInfoSetter setter = contentFolder.transform.GetChild(n).GetComponent<SongListInfoSetter>();

        menu.SetFileName($"{setter.artist}-{setter.title}");

        SceneManager.LoadScene("InGame");
    }

    public void SetSync(string inputed)
    {
        menu.SetSync(inputed);
    }
    public void SetSpeed(string inputed)
    {
        menu.SetSpeed(inputed);
    }
}