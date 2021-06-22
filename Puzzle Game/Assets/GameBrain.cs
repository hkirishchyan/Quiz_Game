using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class GameBrain : MonoBehaviour
{
    public GameObject numberFrame;
    public GameObject mainCanvas;
    public List<Sprite> frameIter = new List<Sprite>();
    HashSet<int> randomSet= new HashSet<int>();
    List<int> randomValues = new List<int>();
    public float bounceTime = 0.25f;
    public float bounceHeight = 10;
    public Text findText;
    public string correctTile;
    int correctTileNum;
    GameObject[] gb;
    public int frameNum;
    bool winGame;
    public GameObject winScreen;
    public Image loadScreen;
    int firstCorrect;
    int secodCorrect;
    // Start is called before the first frame update
    void Start()
    {
        winGame = false;
        frameNum = 3;
        StartCoroutine(Fade());
        StartCoroutine(TileManager());
        firstCorrect=-1;
        secodCorrect=-1;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Restart()
    {
        StartCoroutine(RestartEnum());
    }

    IEnumerator RestartEnum()
    {
        loadScreen.gameObject.SetActive(true);
        Color loadAplha = loadScreen.color;
        while (loadScreen.color.a < 1)
        {
            loadAplha.a += 0.03f;
            loadScreen.color = loadAplha;
            yield return new WaitForSeconds(0.02f);
        }
        winScreen.SetActive(false);
        frameNum = 3;
        randomValues.Clear();
        randomSet.Clear();
        firstCorrect = -1;
        secodCorrect = -1;
        yield return new WaitForSeconds(2);
        while (loadScreen.color.a > 0)
        {
            loadAplha.a -= 0.03f;
            loadScreen.color = loadAplha;
            yield return new WaitForSeconds(0.02f);
        }
        findText.gameObject.SetActive(true);
        winGame = false;      
        StopCoroutine(TileManager());
        StartCoroutine(TileManager());
    }
    IEnumerator CorrectTile()
    {
        yield return new WaitForSeconds(2f);
        print("Correct");
        randomValues.Clear();
        randomSet.Clear();
        foreach (Object obj in gb)
        {
            if (obj != null)
                Destroy(obj);
        }
        frameNum += 3;
        winGame = frameNum == 9;
        StopCoroutine(TileManager());
        StartCoroutine(TileManager());
    }
    public void TileClick(int setTile)
    {
        gb[setTile].transform.Find("Background/Symbol").DOJump(gb[setTile].transform.position, 20, 2, 2);
        print("Clicked Tile: "+setTile);
        if (setTile==correctTileNum&&!winGame)
        {
            StartCoroutine(CorrectTile());
        }
        else if(setTile == correctTileNum && winGame)
        {
            StartCoroutine(GameWin());
            DestroyObject();
            findText.gameObject.SetActive(false);
        }
        else
        {
            gb[setTile].transform.Find("Background/Symbol").DOShakePosition(2.0f, strength: Vector3.one*2, vibrato: 5, randomness: 2, snapping: false, fadeOut: true);
            print("Incorrect");
        }
    }
    IEnumerator GameWin()
    {
        yield return new WaitForSeconds(2f);
        winScreen.SetActive(true);
        Color winAlpha=winScreen.transform.Find("Background").GetComponent<Image>().color;
        while (winAlpha.a < 0.7)
        {
            winAlpha.a += 0.03f;
            winScreen.transform.Find("Background").GetComponent<Image>().color = winAlpha;
            yield return new WaitForSeconds(0.02f);
        }

    }
    void DestroyObject()
    {
        for (int i = 0; i < frameNum; i++)
        {
            Destroy(gb[i],i*0.2f);
        }
    }
    IEnumerator Fade()
    {
        for (float ft = 0f; ft <= 1; ft += 0.1f)
        {
            Color c = findText.color;
            c.a = ft;
            findText.color = c;
            yield return new WaitForSeconds(.05f);
        }
    }
    IEnumerator TileManager()
    {
        loadScreen.gameObject.SetActive(false);
        for (int i = 0; i < 15; i++)
        {
            randomSet.Add(Random.Range(0, 36));
        }
        randomValues = new List<int>(randomSet);
        gb = new GameObject[frameNum];
        Vector3 posHolder = mainCanvas.transform.position + Vector3.up * 150 + Vector3.left * 100;
        float xPos = posHolder.x;

        correctTileNum = Random.Range(0, frameNum);
        //while (firstCorrect == correctTileNum||secodCorrect==correctTileNum)
        //{
        //    if (correctTileNum <= frameNum && correctTileNum>=0)
        //    {
        //        correctTileNum += Random.Range(-1, 1);
        //    }
        //}
        correctTile = frameIter[randomValues[correctTileNum]].name;
        print("Correct Tile: " + correctTile);
        findText.text = $"FIND {correctTile.ToUpper()}";
        print("Correct Tile Number: " + correctTileNum + 1);
        if (firstCorrect == -1)
        {
            firstCorrect = correctTileNum;
        }
        else
        {
            secodCorrect = correctTileNum;
        }
        for (int i = 0; i < frameNum; i++)
        {
            gb[i] = Instantiate(numberFrame, posHolder, numberFrame.transform.rotation, mainCanvas.transform);
            Transform backColor = gb[i].transform.Find("Background");
            Transform symbolHolder = gb[i].transform.Find("Background/Symbol");
            symbolHolder.GetComponent<Image>().sprite = frameIter[randomValues[i]];
            backColor.GetComponent<Image>().color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

            posHolder += Vector3.right * 100;
            if ((i + 1) % 3 == 0)
            {
                posHolder = new Vector3(xPos, posHolder.y, posHolder.z);
                posHolder += Vector3.down * 100;
            }
            int j = i;
            gb[i].GetComponent<Button>().onClick.AddListener(() => { TileClick(j); });
            if (frameNum == 3)
            {
                gb[i].transform.DOPunchScale(Vector3.one / 2, 1, 1, 0);
            }
            //gb[i].transform.DOJump(gb[i].transform.position + Vector3.up * 10, 5, 1, 1);
            gb[i].GetComponent<Button>().interactable = false;
            if (frameNum == 3)
            {
                yield return new WaitForSeconds(.5f);
            }
            else
            {
                yield return new WaitForSeconds(0);
            }
        }
        for (int i = 0; i < frameNum; i++)
        {
            gb[i].GetComponent<Button>().interactable = true;
        }
    }
}
